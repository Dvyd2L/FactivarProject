namespace Interfaces;

public interface ILoginResponse
{
    string Email { get; init; }
    string Token { get; init; }
}