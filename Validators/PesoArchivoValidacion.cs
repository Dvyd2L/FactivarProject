using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Validators;

public class PesoArchivoValidacion(int PesoMaximoEnMegaBytes)
        : ValidationAttribute
{
    #region PROPs
    private readonly int _pesoMaximoEnMegaBytes = PesoMaximoEnMegaBytes;
    #endregion

    #region METHODs
    // value representa al archivo
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Si es null (no viene foto) damos el ok para que se guarde, en este caso, el producto sin imagen
        if (value == null)
        {
            return ValidationResult.Success; // Pasamos la validación porque puede interesarnos guardar un producto sin imagen
        }

        // IFormFile es el dato tal y como entra desde la post

        // Esta segunda comprobación revisa si la foto viene como formFile. Si viene como formFile sigue hacia adelante
        // para revisar el tamaño. Si no, pasamos la validación pero el producto no guardará imagen
        if (value is not IFormFile formFile)
        {
            return ValidationResult.Success;
        }

        // Si sobrepasa el tamaño devolvemos un error
        if (formFile.Length > _pesoMaximoEnMegaBytes * 1024 * 1024)
        {
            return new ValidationResult($"El peso del archivo no debe ser mayor a {_pesoMaximoEnMegaBytes}mb");
        }

        // Si hemos llegado hasta aquí es que todo ha ido bien y el archivo cumple con el tamaño especificado en el DTO
        return ValidationResult.Success;
    }
    #endregion
}