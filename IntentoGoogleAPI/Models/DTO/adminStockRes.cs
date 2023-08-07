namespace IntentoGoogleAPI.Models.DTO
{
    public class adminStockRes
    {
        public int intId { get; set; }
        public string? nombreProducto { get; set; }
        public string? CodigoProducto { get; set; }
        public int? Stock { get; set; }
        public int? stockMinimo { get; set; }
        public string? nombreTienda { get; set; }
        public int idTienda { get; set; }
    }
}
