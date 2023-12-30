using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Helpers;

public class ConfigSetup(WebApplicationBuilder builder)
{
    private readonly bool _isDevelopment = Environment
        .GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

    public string GetConnectionString() => !_isDevelopment
            ? builder.Configuration["CONNECTION_STRING"] ?? "Not set yet"
            : builder.Configuration.GetConnectionString("DefaultConnection") ?? "Not set yet";

    public string GetSecret() => !_isDevelopment
            ? builder.Configuration["ClaveJWT"] ?? "Not set yet"
            : builder.Configuration["ClaveJWT"] ?? "Not set yet";

    public string GetAudience() => !_isDevelopment
        ? builder.Configuration["AudienceJWT"] ?? "Not set yet"
        : builder.Configuration["AudienceJWT"] ?? "Not set yet";

    public string GetIssuer() => !_isDevelopment
        ? builder.Configuration["IssuerJWT"] ?? "Not set yet"
        : builder.Configuration["IssuerJWT"] ?? "Not set yet";
}
