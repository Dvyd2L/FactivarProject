using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using UsersMicroservice.Models;
using Utils;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

#region CONFIGs
string connectionString = new ConfigSetup(builder).GetConnectionString();
string secret = new ConfigSetup(builder).GetSecret();
#endregion CONFIGs

#region SERVICEs
builder.Services
    .AddControllers()
    .AddJsonOptions((o) =>
    {
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddDbContext<UsersContext>(options =>
{
    _ = options.UseSqlServer(connectionString);
    _ = options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
          Encoding.UTF8.GetBytes(secret))
    });

#region CORS Policy
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        // builder.WithOrigins("https://www.MyOcelotApiGw.com").WithMethods("GET").AllowAnyHeader();
        _ = builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});
#endregion CORS Policy

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
#endregion MIDDLEWAREs

app.MapControllers();
app.Run();
