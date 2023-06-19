namespace IntentoGoogleAPI.Models.DTO
{
    public partial class LoginResponse
    {
        public int IntId { get; set; }

        public string Username { get; set; } = null!;

        public string? Correo { get; set; }

        public int? UserType { get; set; }
        public string? JWTToken{ get; set; }
    }
}
