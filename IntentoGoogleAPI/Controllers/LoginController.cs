﻿using IntentoGoogleAPI.Models;
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
            string jwtToken = GenerateToken(cuenta);
            //token
            return Ok(new LoginResponse()
            {
                IntId = cuenta.IntId,
                Username = cuenta.Username,
                Correo = cuenta.Correo,
                UserType = cuenta.UserType,
                IdTienda = cuenta.IdTienda,
                JWTToken = jwtToken,
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
                    return Ok(new { message = "Revise su correo" });
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
