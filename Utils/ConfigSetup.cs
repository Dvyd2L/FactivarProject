using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Utils;

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
}
