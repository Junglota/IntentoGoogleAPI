using System;
using System.Collections.Generic;

namespace IntentoGoogleAPI.Models;

public partial class Usuario
{
    public int IntId { get; set; }

    public string Username { get; set; } = null!;

    public string? Password { get; set; }

    public string? Correo { get; set; }

    public int? UserType { get; set; }

    public virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();

    public virtual ICollection<Tiendum> Tienda { get; set; } = new List<Tiendum>();

    public virtual Tipo? UserTypeNavigation { get; set; }
}
