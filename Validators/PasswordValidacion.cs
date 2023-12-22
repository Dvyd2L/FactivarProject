using System.ComponentModel.DataAnnotations;

namespace Validators;
public class PasswordValidacion : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return new ValidationResult("La contraseña debe estar presente");

        string? password = value as string;

        return password?.Length < 8
            ? new ValidationResult("La contraseña debe tener al menos 8 caracteres de longitud")
            : password?.Any(char.IsUpper) == false
            ? new ValidationResult("La contraseña debe contener al menos una letra mayúscula")
            : password?.Any(char.IsLower) == false
            ? new ValidationResult("La contraseña debe contener al menos una letra minúscula")
            : password?.Any(char.IsDigit) == false
            ? new ValidationResult("La contraseña debe contener al menos un número")
            : password?.Any(char.IsSymbol) == false
            ? new ValidationResult("La contraseña debe contener al menos un símbolo")
            : ValidationResult.Success;
    }
}
