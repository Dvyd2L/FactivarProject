using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Services;
using System.Text;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string basePath = Directory.GetCurrentDirectory();
string ocelotConfigFilePath = Path.Combine(basePath, "Config", "ocelot.json");

#region CONFIGs
builder.Configuration.AddJsonFile(ocelotConfigFilePath, optional: false, reloadOnChange: true);
#endregion CONFIGs

#region SERVICEs
builder.Services.AddControllers();
builder.Services.AddOcelot().AddSingletonDefinedAggregator<ServiceAggregator>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
        };
    });

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

app.UseOcelot().Wait();
app.UseHttpsRedirection();
app.UseAuthorization();
#endregion MIDDLEWAREs

app.MapControllers();

app.Run();
