using BackOffice.Application.Common.Interfaces;

namespace BackOffice.Infrastructure.Security;

/// <summary>
/// Implémentation du service IPasswordHasher utilisant l'algorithme BCrypt.
/// BCrypt est un choix robuste car il est lent et intègre un "sel" (salt) automatiquement,
/// le rendant résistant aux attaques par force brute et tables arc-en-ciel.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    /// <summary>
    /// Hache un mot de passe en utilisant BCrypt.
    /// </summary>
    public string Hash(string password)
    {
        // La méthode HashPassword génère un sel aléatoire et l'intègre directement dans le hash résultant.
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// Vérifie qu'un mot de passe en clair correspond à un hash BCrypt.
    /// </summary>
    public bool Verify(string password, string hashedPassword)
    {
        // La méthode Verify extrait le sel du hash et l'utilise pour hacher le mot de passe fourni,
        // puis compare les deux hashes.
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}