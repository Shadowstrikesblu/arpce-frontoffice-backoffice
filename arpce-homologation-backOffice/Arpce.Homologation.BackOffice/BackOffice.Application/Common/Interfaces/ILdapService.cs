namespace BackOffice.Application.Common.Interfaces;

public interface ILdapService
{
    /// <summary>
    /// Authentifie un utilisateur via LDAP et retourne le code de son profil (lu depuis un attribut AD).
    /// Retourne null si l'authentification échoue.
    /// </summary>
    Task<string?> AuthenticateAndGetProfileCodeAsync(string username, string password);
}