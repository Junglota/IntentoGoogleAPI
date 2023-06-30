using System;
using System.Collections.Generic;

namespace IntentoGoogleAPI.Models;

public partial class Inventario
{
    public int IntId { get; set; }

    public string IdProducto { get; set; } = null!;

    public int? StockMinimo { get; set; }

    public int? Stock { get; set; }

    public int? IdTienda { get; set; }

    internal virtual Producto IdProductoNavigation { get; set; } = null!;
}
