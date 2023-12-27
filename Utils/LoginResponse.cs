using Interfaces;

namespace Helpers;
public record LoginResponse(string Email, string Token) : ILoginResponse;
