using System;
using System.Collections.Generic;

namespace IntentoGoogleAPI.Models;

public partial class Usuario
{
    public int IntId { get; set; }

    public string? Nombre { get; set; }

    public string? Apellido { get; set; }

    public string Username { get; set; } = null!;

    public string? Password { get; set; }

    public string? Correo { get; set; }

    public int? UserType { get; set; }

    public int? IdTienda { get; set; }

    internal string? EToken { get; set; }

    internal DateTime? ETokenValidUntil { get; set; }

    internal virtual Tiendum? IdTiendaNavigation { get; set; }

    internal virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();

    internal virtual ICollection<Tiendum> Tienda { get; set; } = new List<Tiendum>();

    internal virtual Tipo? UserTypeNavigation { get; set; }
}
