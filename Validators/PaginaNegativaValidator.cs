using System.ComponentModel.DataAnnotations;

namespace Validators;

public class PaginaNegativaValidator : ValidationAttribute
{
    public PaginaNegativaValidator() { }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return new ValidationResult($"El numero de paginas no puede ser nulo");

        int? paginas = value as int?;

        return paginas < 0
             ? new ValidationResult($"El numero de paginas no puede ser negativo")
             : ValidationResult.Success;
    }
}