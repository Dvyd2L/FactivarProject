using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Validators;

public class TipoArchivoValidacion : ValidationAttribute
{
    private readonly string[] tiposValidos;

    public TipoArchivoValidacion(string[] tiposValidos)
        => this.tiposValidos = tiposValidos;

    // Desde el DTO le especificamos qué tipo de archivo es el que vamos a elegir (en este caso imagen)
    public TipoArchivoValidacion(GrupoTipoArchivo grupoTipoArchivo)
        => tiposValidos = grupoTipoArchivo == GrupoTipoArchivo.Imagen
            ? ["image/jpeg", "image/png", "image/gif"]
            : [];

    // value representa al archivo
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }

        // IFormFile es el dato tal y como entra desde la post
        if (value is not IFormFile formFile)
        {
            return ValidationResult.Success;
        }

        // ContentType deberá ser uno de los tiposValidos { "image/jpeg", "image/png", "image/gif" } para pasar la validación
        return !tiposValidos.Contains(formFile.ContentType)
            ? new ValidationResult($"El tipo del archivo debe ser uno de los siguientes: {string.Join(", ", tiposValidos)}")
            : ValidationResult.Success;
    }
}
