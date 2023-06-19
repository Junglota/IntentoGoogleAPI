using System;
using System.Collections.Generic;

namespace IntentoGoogleAPI.Models;

public partial class Inventario
{
    public string IdProducto { get; set; } = null!;

    public int? StockMinimo { get; set; }

    public int? Stock { get; set; }

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
