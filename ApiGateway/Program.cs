using Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Middlewares;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Services;
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

        // TO DO: esto aun no funciona
        options.Events = new JwtBearerEvents
        {
            //OnAuthenticationFailed = context =>
            //{
            //    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            //    {
            //        context.Response.Headers.Append("Token-Expired", "true");
            //    }

            //    if (context.Exception.GetType() == typeof(SecurityTokenInvalidAudienceException))
            //    {
            //        context.Response.Headers.Append("Token-Audience", "Invalid");
            //    }

            //    if (context.Exception.GetType() == typeof(SecurityTokenException))
            //    {
            //        context.Response.Headers.Append("Token-Exception", true.ToString());
            //    }

            //    return Task.CompletedTask;
            //},

            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                context.Response.Headers.Append("WWW-Authenticate", @"Bearer error = ""invalid_token""");

                // Personaliza tu mensaje de error aquí
                string json = JsonSerializer.Serialize(new { error = "Mensaje de error personalizado" });
                return context.Response.WriteAsync(json);
            }
        };
    });

#region CORS Policy
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        _ = builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });

    options.AddPolicy("DevCorsPolicy", builder =>
    {
        _ = builder.WithOrigins("https://www.localhost:4200").AllowAnyMethod().AllowAnyHeader();
    });

    options.AddPolicy("ProdCorsPolicy", builder =>
    {
        // builder.WithOrigins("https://www.MyOcelotApiGw.com").WithMethods("GET").AllowAnyHeader();
        _ = builder.WithOrigins("https://www.hosting.MyOcelotApiGw.com").AllowAnyMethod().AllowAnyHeader();
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
}

app.UseMiddleware<PreflightRequestMiddleware>();
app.UseMiddleware<LogRequestMiddleware>();
app.UseOcelot().Wait();
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
#endregion MIDDLEWAREs

app.MapControllers();

app.Run();
