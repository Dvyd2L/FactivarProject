using Helpers.Enums;

namespace DTOs.UsersMS;

public class UserDTO
{
    public required string Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string? Telefono { get; set; }

    public string? AvatarUrl { get; set; }

    public string Email { get; set; } = null!;

    public EnumRoles Rol { get; set; }
}
