using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using ToolBook.Server.Models;

namespace ToolBook.Server.Services;

public class JwtTokenService(IConfiguration config)
{
    public string CreateToken(User user)
    {
        // Opretter claims med de brugeroplysninger, som skal gemmes i tokenet.
        // De kan senere bruges til fx at identificere brugeren og kontrollere rollen.
        var id = new Claim(ClaimTypes.NameIdentifier, user.Id.ToString());
        var name = new Claim(ClaimTypes.Name, user.Name);
        var email = new Claim(ClaimTypes.Email, user.Email);
        var role = new Claim(ClaimTypes.Role, user.Role.ToString());

        var claims = new List<Claim> { id, name, email, role };

        // Henter JWT-indstillinger.
        // Key kommer fra User Secrets, mens Issuer, Audience og udløbstid kommer fra appsettings.json.
        var key = config["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT key mangler");
        
        var issuer = config["Jwt:Issuer"]
            ?? throw new InvalidOperationException("JWT Issuer mangler");
        
        var audience = config["Jwt:Audience"]
            ?? throw new InvalidOperationException("JWT audience mangler");

        // Konverterer tokenets levetid fra konfigurationen til minutter
        // og sikrer at værdien er gyldig.
        var parsedExpiresMinutes = int.TryParse( config["Jwt:ExpiresMinutes"], out int result);
        if (!parsedExpiresMinutes || result <= 0)
        {
            throw new InvalidOperationException("JWT expiresMinutes mangler eller kunne ikke konverteres til et positivt int");
        }

        var expiresAt = DateTime.UtcNow.AddMinutes(result);
        
        // Konverterer den Base64-gemte secret key til en signing key
        // og bruger HMAC SHA-256 til at signere tokenet.
        byte[] byteKey = Convert.FromBase64String(key);
        var securityKey = new SymmetricSecurityKey(byteKey);
        var algorithm = SecurityAlgorithms.HmacSha256;
        
        var signingCred = new SigningCredentials(securityKey, algorithm);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: signingCred
        );

        // Konverterer JWT-objektet til den token-string, der sendes tilbage til clienten.
        var tokenToString = new JwtSecurityTokenHandler();
        return tokenToString.WriteToken(token);
    }
}