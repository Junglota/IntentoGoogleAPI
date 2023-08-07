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

    public string? EToken { get; set; }

    public DateTime? ETokenValidUntil { get; set; }

    public int? Estado { get; set; }

    public virtual Estado? EstadoNavigation { get; set; }

    public virtual Tiendum? IdTiendaNavigation { get; set; }

    internal virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();

    internal virtual ICollection<Tiendum> Tienda { get; set; } = new List<Tiendum>();

    internal virtual Tipo? UserTypeNavigation { get; set; }
}
