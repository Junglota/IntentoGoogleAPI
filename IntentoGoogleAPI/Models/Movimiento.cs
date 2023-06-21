using System;
using System.Collections.Generic;

namespace IntentoGoogleAPI.Models;

public partial class Movimiento
{
    public int IntId { get; set; }

    public DateTime? Fecha { get; set; }

    public string? IdProducto { get; set; }

    public int? Cantidad { get; set; }

    public string? TipoMovimiento { get; set; }

    public string? Descripcion { get; set; }

    public int? Usuario { get; set; }

    public int? IdTienda { get; set; }

    public virtual Descripcion? DescripcionNavigation { get; set; }

    public virtual Producto? IdProductoNavigation { get; set; }

    public virtual Tiendum? IdTiendaNavigation { get; set; }

    public virtual Usuario? UsuarioNavigation { get; set; }
}
