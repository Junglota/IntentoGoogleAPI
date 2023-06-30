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

    internal virtual Descripcion? DescripcionNavigation { get; set; }

    internal virtual Producto? IdProductoNavigation { get; set; }

    internal virtual Tiendum? IdTiendaNavigation { get; set; }

    internal virtual Usuario? UsuarioNavigation { get; set; }
}
