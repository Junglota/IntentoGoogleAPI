using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace IntentoGoogleAPI.Models;

public partial class Producto
{
    public string IdProducto { get; set; } = null!;

    public string? Nombre { get; set; }

    public decimal? Precio { get; set; }

    public string? Categoria { get; set; }

    public int? IdTienda { get; set; }
    [JsonIgnore]
    internal virtual ICollection<Inventario> Inventarios { get; set; } = new List<Inventario>();
    [System.Text.Json.Serialization.JsonIgnore]
    internal virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}
