using System;
using System.Collections.Generic;

namespace IntentoGoogleAPI.Models;

public partial class Venta
{
    public int IntId { get; set; }

    public string? Familia { get; set; }

    public string? Grupo { get; set; }

    public string? CodProducto { get; set; }

    public int? CantidadCarryOut { get; set; }

    public int? CantidadDelivery { get; set; }

    public int? CantidadAgregadores { get; set; }

    public DateTime? FechaVenta { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public int? IdHeladeria { get; set; }

    public virtual Producto? CodProductoNavigation { get; set; }

    public virtual Tiendum? IdHeladeriaNavigation { get; set; }
}
