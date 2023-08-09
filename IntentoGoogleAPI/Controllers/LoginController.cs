using IntentoGoogleAPI.Models;
using IntentoGoogleAPI.Models.DTO;
using IntentoGoogleAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IntentoGoogleAPI.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration config;
        private readonly LoginService loginService;
        private readonly ContabilidadContext context;
        public LoginController(LoginService loginService, IConfiguration config, ContabilidadContext context)
        {
            this.loginService = loginService;
            this.config = config;
            this.context = context;
        }
        // POST api/<LoginController>
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginCred loginCred)
        {
            var cuenta = await loginService.GetUsuario(new LoginCred { Username = loginCred.Username, Password = loginCred.Password});
            if (cuenta is null)
            {
                return BadRequest(new { message = "No hay coincidencias." });
            }
            if(cuenta.Estado == 2)
            {
                return BadRequest(new { message = "Cuenta inactiva" });
            }
            string jwtToken = GenerateToken(cuenta);
            //token
            return Ok(new LoginResponse()
            {
                IntId = cuenta.IntId,
                Nombre = cuenta.Nombre,
                Apellido = cuenta.Apellido,
                Username = cuenta.Username,
                Correo = cuenta.Correo,
                UserType = cuenta.UserType,
                IdTienda = cuenta.IdTienda,
                JWTToken = jwtToken,
                Estado = cuenta.Estado,
            });
        }
        [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPassword([FromBody]ForgotPassword correo)
        {
            var cuenta = await loginService.GetUsuarioByMail(correo.correo);
            if (cuenta is null)
            {
                return BadRequest(new { message = "Correo no encontrado" });
            }
            Random rnd = new Random();
            var num = rnd.Next(1000, 10000).ToString();
            var eToken =GetSHA256(num);
            bool confirmacion = loginService.EnviarCorreo(cuenta,num);
            if (confirmacion)
            {
                cuenta.EToken = eToken;
                cuenta.ETokenValidUntil = DateTime.Now.AddMinutes(30);
                context.Entry(cuenta).State = EntityState.Modified;
                try
                {
                    await context.SaveChangesAsync();
                    return Ok(new { message = "Revise su correo"});
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Error guardando el token en la base de datos");
                }   
            }
            return BadRequest(new { message = "Hubo un error" });
        }
        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPassword resetPassword)
        {
            var encryptToken = GetSHA256(resetPassword.eToken);
            var cuenta = await loginService.GetUsuarioByToken(encryptToken);
            if (cuenta is null) 
            {
                return BadRequest(new { message = "Cuenta no encontrada" });
            }
            if(cuenta.ETokenValidUntil <= DateTime.Now)
            {
                return BadRequest("El codigo ya expiro");
            }
            cuenta.Password = resetPassword.NewPassword;
            context.Entry(cuenta).State = EntityState.Modified;
            try
            {
                await context.SaveChangesAsync();
                return Ok(new { message = "Contraseña modificada" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Error al guardar la contraseña" });
            }

        }


        [HttpPost("etokencheck")]
        public async Task<IActionResult> eTokenCheck([FromBody]eTokenCheck eToken)
        {
            var encryptToken = GetSHA256(eToken.eToken);
            var cuenta = await loginService.GetUsuarioByToken(encryptToken);
            if (cuenta is null) 
            {
                return BadRequest(new { message = "Cuenta no encontrada" });
            }
            if(cuenta.ETokenValidUntil <= DateTime.Now)
            {
                return BadRequest("El codigo ya expiro");
            }
                return Ok();
        }

        [HttpPost("Registro")]
        public async Task<IActionResult> Registro([FromBody] Registro registro)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    if (context.Usuarios == null)
                    {
                        return Problem("Entity set 'ContabilidadContext.Productos' is null.");
                    }

                    Usuario usuario = new Usuario()
                    {
                        Nombre = registro.Nombre,
                        Apellido = registro.Apellido,
                        Username = registro.Username,
                        Password = registro.Password,
                        Correo = registro.Correo,
                        UserType = 2
                    };

                    context.Usuarios.Add(usuario);
                    await context.SaveChangesAsync();

                    Tiendum tienda = new Tiendum()
                    {
                        Localidad = registro.Localidad,
                        IdPropietario = usuario.IntId
                    };

                    context.Tienda.Add(tienda);
                    await context.SaveChangesAsync();

                    usuario.IdTienda = tienda.IntId;
                    context.Entry(usuario).State = EntityState.Modified;
                    await context.SaveChangesAsync();

                    transaction.Commit();

                    return Created("registro", usuario);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return BadRequest(ex);
                }
            }
        }

        [HttpPost("updatemail")]
        public async Task<IActionResult> updateMail([FromBody] updateMail updateMail)
        {
            var cuenta = await context.Usuarios.FirstOrDefaultAsync(u=> u.Correo == updateMail.correo && u.Password == updateMail.password);
            if (cuenta is null)
            {
                return BadRequest(new { message = "Cuenta no encontrada" });
            }
            cuenta.Correo = updateMail.newCorreo;

            try
            {
                await context.SaveChangesAsync();
            }
            catch
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpPost("updatepassword")]
        public async Task<IActionResult> updatePassword([FromBody] updatePassword updatePassword)
        {
            var cuenta = await context.Usuarios.FirstOrDefaultAsync(u => u.Correo == updatePassword.correo && u.Password == updatePassword.password);
            if (cuenta is null)
            {
                return BadRequest(new { message = "Cuenta no encontrada" });
            }
            cuenta.Password = updatePassword.newPassword;

            try
            {
                await context.SaveChangesAsync();
            }
            catch
            {
                return BadRequest();
            }

            return Ok();
        }

        [NonAction]
        public string GenerateToken(Usuario usuario)
        {
            var Claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.Username),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim("UserType", usuario.UserType.ToString()),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("JWT:Key").Value));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var securityToken = new JwtSecurityToken(claims: Claims, expires: DateTime.Now.AddMinutes(60), signingCredentials: cred);
            string token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return token;
        }
        [NonAction]
        public static string GetSHA256(string str)
        {
            SHA256 sHA256 = SHA256Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sHA256.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();

        }
    }
}
