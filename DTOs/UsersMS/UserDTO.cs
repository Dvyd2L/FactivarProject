using Validators;

namespace DTOs.UsersMS;

public class UserDTO
{
    [EmailValidator]
    public required string Email { get; set; }

    [PasswordValidator]
    public required string Password { get; set; }
}
