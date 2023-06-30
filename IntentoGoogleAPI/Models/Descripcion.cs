using System;
using System.Collections.Generic;

namespace IntentoGoogleAPI.Models;

public partial class Descripcion
{
    public string Descripcion1 { get; set; } = null!;

    internal virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}
