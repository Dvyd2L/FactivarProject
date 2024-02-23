using Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Middlewares;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Services;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

#region CONFIGs
string basePath = Directory.GetCurrentDirectory();
string ocelotConfigFilePath = Path.Combine(basePath, "Properties", "ocelot.json");
builder.Configuration.AddJsonFile(ocelotConfigFilePath, optional: false, reloadOnChange: true);

ConfigSetup cs = new(builder);
string secret = cs.GetSecret();
string audience = cs.GetAudience();
string issuer = cs.GetIssuer();
SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(secret));
#endregion CONFIGs

#region SERVICEs
builder.Services.AddControllers();
builder.Services.AddOcelot().AddSingletonDefinedAggregator<ServiceAggregator>();

builder.Services.AddTransient<PreflightRequestMiddleware>();
builder.Services.AddTransient<LogRequestMiddleware>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
    options.TokenValidationParameters = new TokenValidationParameters
    {
      ValidateLifetime = true,
      ValidAudience = audience,
      ValidateAudience = true, // en mis ejemplos es false
      ValidateIssuer = true, // en mis ejemplos es false
      ValidIssuer = issuer,
      ValidateIssuerSigningKey = true,
      IssuerSigningKey = securityKey
    };

    // TODO: esto aun no funciona
    options.Events = new JwtBearerEvents
    {
      OnAuthenticationFailed = context =>
      {
        Dictionary<Type, string[]> exceptionToHeaderMap = new()
        {
          { typeof(SecurityTokenExpiredException), ["Token-Expired", "true"] },
          { typeof(SecurityTokenInvalidAudienceException), ["Token-Audience", "Invalid"] },
          { typeof(SecurityTokenException), ["Token-Exception", true.ToString()] }
        };

        if (exceptionToHeaderMap.TryGetValue(context.Exception.GetType(), out string[]? header))
        {
          context.Response.Headers.Append(header[0], header[1]);
        }

        return Task.CompletedTask;
      },

      OnChallenge = context =>
      {
        context.HandleResponse();
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = MediaTypeNames.Application.Json;
        context.Response.Headers.Append(HeaderNames.WWWAuthenticate, @"Bearer error = ""invalid_token""");

        // Personaliza tu mensaje de error aquí
        string json = JsonSerializer.Serialize(new { error = "Mensaje de error personalizado" });
        return context.Response.WriteAsync(json);
      }
    };
  })
  // TODO: Cookies
  .AddCookie((options) =>
  {
    options.Cookie.HttpOnly = true; // Este atributo hace que la cookie sea inaccesible para el código del lado del cliente, como JavaScript.
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Este atributo indica que la cookie solo debe enviarse a través de HTTPS.
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax; // Este atributo puede prevenir ciertos tipos de ataques de falsificación de solicitudes entre sitios.
    options.Cookie.Name = "MiCookie"; // Este atributo define el nombre de la cookie.
    options.ExpireTimeSpan = TimeSpan.FromDays(7); // Este atributo define cuándo expira la cookie.
  })
  .AddIdentityCookies((options) => { });

#region CORS Policy
builder.Services.AddCors(options =>
{
  string? origins = builder.Configuration.GetSection("AllowedHosts").ToString();
  options.AddDefaultPolicy(builder =>
    {
      _ = builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });

  options.AddPolicy("DevCorsPolicy", builder =>
  {
    // TODO: cambiar configuracion desde app settings
    _ = builder.WithOrigins(origins!).AllowAnyMethod().AllowAnyHeader();
  });

  options.AddPolicy("ProdCorsPolicy", builder =>
  {
    // TODO: cambiar configuracion desde app settings
    _ = builder.WithOrigins(origins!).AllowAnyMethod().AllowAnyHeader();
  });
});
#endregion CORS Policy

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion SERVICEs

WebApplication app = builder.Build();

#region MIDDLEWAREs
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  _ = app.UseSwagger();
  _ = app.UseSwaggerUI();
  //_ = app.UseCors("DevCorsPolicy");
  _ = app.UseCors();
}
else
{
  _ = app.UseCors("ProdCorsPolicy");
}

app.UseMiddleware<PreflightRequestMiddleware>();
app.UseMiddleware<LogRequestMiddleware>();
app.UseOcelot().Wait();
app.UseHttpsRedirection();
app.UseAuthorization();
// app.UseAuthentication(); // esta no estaba aquí originalmente
#endregion MIDDLEWAREs

app.MapControllers();

app.Run();
