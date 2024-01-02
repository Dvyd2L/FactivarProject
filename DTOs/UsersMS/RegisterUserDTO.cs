using Microsoft.AspNetCore.Http;
using Validators;

namespace DTOs.UsersMS;

public class RegisterUserDTO
{
    public required string Nombre { get; set; }

    public required string Apellidos { get; set; }

    public string? Telefono { get; set; }

    [PesoArchivoValidacion(PesoMaximoEnMegaBytes: 5)]
    [TipoArchivoValidacion(grupoTipoArchivo: GrupoTipoArchivo.Imagen)]
    public IFormFile? Avatar { get; set; }

    [EmailValidator]
    public required string Email { get; set; }

    [PasswordValidator]
    public required string Password { get; set; }
}
