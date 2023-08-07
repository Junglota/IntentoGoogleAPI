using System;
using System.Collections.Generic;

namespace IntentoGoogleAPI.Models;

public partial class Tiendum
{
    public int IntId { get; set; }

    public int? IdLocalidad { get; set; }

    public string? Localidad { get; set; }

    public int? IdPropietario { get; set; }

    public virtual Usuario? IdPropietarioNavigation { get; set; }

    internal virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();

    internal virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
