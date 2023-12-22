using System.ComponentModel.DataAnnotations;

namespace Validators;

public class PrecioValidacion : ValidationAttribute
{
    public PrecioValidacion() { }

    // value representa al archivo
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return new ValidationResult($"El precio debe estar presente");
        }

        decimal? precio = value as decimal?;

        return precio < 0
            ? new ValidationResult($"El precio no puede ser negativo")
            : ValidationResult.Success;
    }
}
