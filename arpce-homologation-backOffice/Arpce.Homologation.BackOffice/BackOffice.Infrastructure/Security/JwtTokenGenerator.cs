using BackOffice.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackOffice.Infrastructure.Security;

/// <summary>
/// Implémentation du service IJwtTokenGenerator.
/// Génère des tokens JWT en utilisant les paramètres (secret, issuer, audience)
/// définis dans la configuration de l'application (appsettings.json).
/// </summary>
public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Génère un token JWT avec les claims essentiels de l'utilisateur.
    /// </summary>
    public string GenerateToken(Guid userId, string userAccount)
    {
        // Création des "claims" 
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            
            new Claim(JwtRegisteredClaimNames.Name, userAccount),
            
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Récupération de la clé secrète depuis la configuration et encodage en bytes.
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]));

        // Création des crédentials de signature avec l'algorithme HMAC SHA256.
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Définition de la date d'expiration du token (par exemple, 1 heure à partir de maintenant).
        var expires = DateTime.UtcNow.AddHours(1);

        // Création de l'objet token avec tous ses paramètres.
        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        // Sérialisation du token en une chaîne de caractères compacte (le token final).
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}