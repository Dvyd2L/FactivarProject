using Validators;

namespace DTOs.UsersMS;

public class LoginUserDTO
{
    [EmailValidator]
    public required string Email { get; set; }

    [PasswordValidator]
    public required string Password { get; set; }
}
