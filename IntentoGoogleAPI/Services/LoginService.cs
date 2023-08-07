using IntentoGoogleAPI.Models.DTO;
using IntentoGoogleAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using MimeKit;

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
        public async Task<Usuario?> GetUsuarioByMail(string correo)
        {
            return await _context.Usuarios.SingleOrDefaultAsync(u => u.Correo == correo);
        }
        public async Task<Usuario?> GetUsuarioByToken(string? eToken)
        {
            return await _context.Usuarios.SingleOrDefaultAsync(u => u.EToken == eToken);
        }
        
        public bool EnviarCorreo(Usuario? usuario,string? eToken)
        {
            string EmailOrigen = "contabilidadsinrebu@gmail.com";
            string EmailDestino = usuario.Correo;
            string Password = "ettqymtesaldzmmi";//"Contabilidadsinrebu123123";
            string codigoVeridicacion = "1234";
                                                                     //Aqui va EmailDestino
            MailMessage mailMessage = new MailMessage(EmailOrigen, "contabilidadsinrebu@gmail.com", "Cambio de contraseña", $"Su codigo de verificacion para cambiar la contraseña: <strong>{eToken}</strong>. El codigo va a ser valido por 30 minutos");
            mailMessage.IsBodyHtml = true;

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Port = 587;
            smtpClient.Credentials = new System.Net.NetworkCredential(EmailOrigen, Password);
            try
            {
                smtpClient.Send(mailMessage);
                smtpClient.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return false;
            }




        }
        public bool EnviarCorreoAlerta(string? correo, string? productos)
        {
            string EmailOrigen = "contabilidadsinrebu@gmail.com";
            string EmailDestino = correo;
            string Password = "ettqymtesaldzmmi";//"Contabilidadsinrebu123123";
            string codigoVeridicacion = "1234";
            //Aqui va EmailDestino
            MailMessage mailMessage = new MailMessage(EmailOrigen, "contabilidadsinrebu@gmail.com", "Alerta de stock bajo", $"Saludos, le informamos que su producto {productos}, se esta agotando.");
            mailMessage.IsBodyHtml = true;

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Port = 587;
            smtpClient.Credentials = new System.Net.NetworkCredential(EmailOrigen, Password);
            try
            {
                smtpClient.Send(mailMessage);
                smtpClient.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return false;
            }




        }
    } 
}
