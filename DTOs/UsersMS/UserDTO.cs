using Validators;

namespace DTOs.UsersMS;

public class UserDTO
{
    [EmailValidator]
    public required string Email { get; set; }

    [PasswordValidacion]
    public required string Password { get; set; }
}
