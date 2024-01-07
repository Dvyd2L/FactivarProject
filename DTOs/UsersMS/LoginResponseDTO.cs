using DTOs.Interfaces;

namespace DTOs.UsersMS;

public record LoginResponse(
    string Id,
    string Nombre,
    string? AvatarUrl,
    string Email,
    string Token
    ) : ILoginResponse;
