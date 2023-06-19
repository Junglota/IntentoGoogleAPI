using IntentoGoogleAPI.Models;
using IntentoGoogleAPI.Models.DTO;
using IntentoGoogleAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IntentoGoogleAPI.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration config;
        private readonly LoginService loginService;
        public LoginController(LoginService loginService, IConfiguration config)
        {
            this.loginService = loginService;
            this.config = config;
        }
        // GET: api/<LoginController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<LoginController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<LoginController>
        [HttpPost("authenticate")]
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
                JWTToken = jwtToken,
            });
        }

        // PUT api/<LoginController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<LoginController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
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
    }
}
