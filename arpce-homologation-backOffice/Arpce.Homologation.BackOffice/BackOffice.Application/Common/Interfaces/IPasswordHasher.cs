namespace BackOffice.Application.Common.Interfaces;

/// <summary>
/// Définit un contrat pour un service de hachage et de vérification de mots de passe.
/// L'abstraction permet de changer facilement l'algorithme de hachage si nécessaire.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hache un mot de passe en clair.
    /// </summary>
    /// <param name="password">Le mot de passe à hacher.</param>
    /// <returns>Le hash du mot de passe.</returns>
    string Hash(string password);

    /// <summary>
    /// Vérifie si un mot de passe en clair correspond à un hash existant.
    /// </summary>
    /// <param name="password">Le mot de passe en clair à vérifier.</param>
    /// <param name="hashedPassword">Le hash contre lequel vérifier.</param>
    /// <returns>True si le mot de passe correspond, sinon False.</returns>
    bool Verify(string password, string hashedPassword);
}