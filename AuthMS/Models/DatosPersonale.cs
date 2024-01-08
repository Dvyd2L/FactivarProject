using System;
using System.Collections.Generic;

namespace UsersMicroservice.Models;

public partial class DatosPersonale
{
    public Guid Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string? Telefono { get; set; }

    public string? AvatarUrl { get; set; }

    public string Email { get; set; } = null!;

    public virtual Credenciale? Credenciale { get; set; }
}
