using System;
using System.Collections.Generic;

namespace IntentoGoogleAPI.Models;

public partial class Estado
{
    public int IntId { get; set; }

    public string? Estado1 { get; set; }

    internal virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
