namespace BackOffice.Application.Common.Interfaces;

/// <summary>
/// Définit un contrat pour un service de génération de tokens d'authentification JWT.
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Génère un token JWT pour un utilisateur spécifique.
    /// </summary>
    /// <param name="userId">L'identifiant unique de l'utilisateur.</param>
    /// <param name="userAccount">Le nom de compte (login) de l'utilisateur.</param>
    /// <returns>Une chaîne de caractères représentant le token JWT.</returns>
    string GenerateToken(Guid userId, string userAccount);
}