using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Validators;
public class EmailValidator : ValidationAttribute
{
    private static readonly Regex EmailRegex = new(@"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$");

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) =>
        value is not string email
            ? new ValidationResult("El correo electrónico es obligatorio")
            : !email.Contains('@')
            ? new ValidationResult("El correo electrónico debe contener @")
            : email.Length < 5
            ? new ValidationResult("El correo electrónico debe tener al menos 5 caracteres")
            : email.Length > 50
            ? new ValidationResult("El correo electrónico debe tener como máximo 50 caracteres")
            : !EmailRegex.IsMatch(email)
            ? new ValidationResult("El correo electrónico debe coincidir con el patrón de la expresión regular")
            : ValidationResult.Success;
}
