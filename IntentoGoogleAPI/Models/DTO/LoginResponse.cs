namespace IntentoGoogleAPI.Models.DTO
{
    public partial class LoginResponse
    {
        public int IntId { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }

        public string Username { get; set; } = null!;

        public string? Correo { get; set; }

        public int? UserType { get; set; }
        public int? IdTienda { get; set; }
        public int? Estado { get; set; }
        public string? JWTToken{ get; set; }
    }
}
