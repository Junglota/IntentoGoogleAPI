using System;
using System.Collections.Generic;

namespace IntentoGoogleAPI.Models;

public partial class Producto
{
    public string Cod { get; set; } = null!;

    public string? Nombre { get; set; }

    public decimal? Precio { get; set; }

    public int? Cantidad { get; set; }

    public int? IdTienda { get; set; }

    public virtual Tiendum? IdTiendaNavigation { get; set; }

    public virtual ICollection<Inventario> Inventarios { get; set; } = new List<Inventario>();

    public virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}
