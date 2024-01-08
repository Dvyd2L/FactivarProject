using Filters;
using Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Middlewares;
using Services;
using Services.Interfaces;
using System.Text;
using System.Text.Json.Serialization;
using UsersMicroservice.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

#region CONFIGs
ConfigurationManager configuration = builder.Configuration;
ConfigSetup cs = new(builder);
string connectionString = cs.GetConnectionString();
string secret = cs.GetSecret();
string audience = cs.GetAudience();
string issuer = cs.GetIssuer();
SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(secret));
#endregion CONFIGs

#region SERVICEs
IServiceCollection services = builder.Services;

services
    .AddControllers(
    (option) =>
    {
        _ = option.Filters.Add<ExceptionFilter>();
    })
    .AddJsonOptions((o) =>
    {
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

services.AddDbContext<UsersContext>(options =>
{
    _ = options.UseSqlServer(connectionString);
    _ = options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

services.AddHttpContextAccessor();

services.AddTransient<IFileHandler, LocalFileService>();
services.AddTransient<IDbService<DatosPersonale, Guid>, DbService<UsersContext, DatosPersonale, Guid>>();
services.AddTransient<IDbService<Credenciale, Guid>, DbService<UsersContext, Credenciale, Guid>>();
services.AddTransient<IHashService, HashService>();
services.AddTransient<TokenService>();
services.AddTransient<LogRequestMiddleware>();

#region AUTHENTICATION
services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = configuration["Authentication:Google:ClientId"] ?? "Not set yet";
        googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"] ?? "Not set yet";
    })
    .AddJwtBearer(options =>
    {
        TokenValidationParameters tvp = new()
        {
            ValidateLifetime = true,
            ValidAudience = audience,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = securityKey
        };

        options.TokenValidationParameters = tvp;
    });
#endregion AUTHENTICATION

#region CORS Policy
services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        // builder.WithOrigins("https://www.MyOcelotApiGw.com").WithMethods("GET").AllowAnyHeader();
        _ = builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});
#endregion CORS Policy

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
});
#endregion SERVICEs

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
#region MIDDLEWAREs
if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}
else if (app.Environment.IsProduction())
{
    _ = app.UseFileServer();
}

app.UseMiddleware<LogRequestMiddleware>();
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication(); // revisar su utilidad
app.UseAuthorization();
#endregion MIDDLEWAREs

app.MapControllers();
app.Run();
