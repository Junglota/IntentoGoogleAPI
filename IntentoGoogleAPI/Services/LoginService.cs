using IntentoGoogleAPI.Models.DTO;
using IntentoGoogleAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace IntentoGoogleAPI.Services
{
    public class LoginService
    {
        private readonly ContabilidadContext _context;
        public LoginService(ContabilidadContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> GetUsuario(LoginCred loginCred)
        {
            return await _context.Usuarios.SingleOrDefaultAsync(m => m.Username == loginCred.Username && m.Password == loginCred.Password);
        }
    }
}
