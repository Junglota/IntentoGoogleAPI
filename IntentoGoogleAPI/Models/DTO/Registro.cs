namespace IntentoGoogleAPI.Models.DTO
{
    public class Registro
    {
        public int IntId { get; set; }

        public string? Nombre { get; set; }

        public string? Apellido { get; set; }

        public string Username { get; set; } = null!;

        public string? Password { get; set; }

        public string? Correo { get; set; }

        public string? Localidad { get; set; }
    }
}
