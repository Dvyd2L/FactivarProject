namespace AuthMS.Models;

public partial class Credenciale
{
    public Guid IdUsuario { get; set; }

    public string Password { get; set; } = null!;

    public byte[]? Salt { get; set; }

    public int Roles_IdRol { get; set; }

    public string? RefreshToken { get; set; }

    public string? EnlaceCambioPass { get; set; }

    public DateTime? FechaEnvioEnlace { get; set; }

    public virtual DatosPersonale IdUsuarioNavigation { get; set; } = null!;

    public virtual Role Roles_IdRolNavigation { get; set; } = null!;
}
