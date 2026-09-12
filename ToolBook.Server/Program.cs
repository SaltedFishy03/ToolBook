using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ToolBook.Server.Data;
using Scalar.AspNetCore;
using ToolBook.Server.Models;
using ToolBook.Server.Services;
using ToolBook.Server.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Authentication services
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<JwtTokenService>();

// CRUD services
builder.Services.AddScoped<IToolCategoryService, ToolCategoryService>();
builder.Services.AddScoped<IToolService, ToolService>();
builder.Services.AddScoped<IToolTypeService, ToolTypeService>();


// Henter JWT-konfigurationen.
// Key ligger i User Secrets, mens Issuer og Audience ligger i appsettings.json.
var key = builder.Configuration["Jwt:Key"]
          ?? throw new InvalidOperationException("JWT key mangler");

var issuer = builder.Configuration["Jwt:Issuer"]
             ?? throw new InvalidOperationException("JWT issuer mangler");

var audience = builder.Configuration["Jwt:Audience"]
               ?? throw new InvalidOperationException("JWT audience mangler");

// Fortæller ASP.NET at JWT Bearer bruges til at identificere brugeren.
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Konverterer den Base64-gemte secret key til en nøgle,
        // som JWT kan bruge til at validere tokenets signatur.
        byte[] byteKey = Convert.FromBase64String(key);
        var convertedKey = new SymmetricSecurityKey(byteKey);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Kontrollerer at tokenet er udstedt af vores server
            ValidateIssuer = true,
            ValidIssuer = issuer,

            // Kontrollerer at tokenet er beregnet til vores client
            ValidateAudience = true,
            ValidAudience = audience,

            // Afviser tokenet hvis det er udløbet
            ValidateLifetime = true,

            // Kontrollerer at tokenets signatur er lavet med vores secret key
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = convertedKey
        };
    });

// Authorization
builder.Services.AddAuthorization();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));


builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();