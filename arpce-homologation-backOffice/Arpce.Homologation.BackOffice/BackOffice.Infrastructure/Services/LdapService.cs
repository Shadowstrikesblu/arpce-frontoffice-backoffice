using BackOffice.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Novell.Directory.Ldap;

namespace BackOffice.Infrastructure.Services;

public class LdapService : ILdapService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<LdapService> _logger;

    public LdapService(IConfiguration configuration, ILogger<LdapService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public Task<string?> AuthenticateAndGetProfileCodeAsync(string username, string password)
    {
        // Validation de base
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return Task.FromResult<string?>(null);

        var ldapHost = _configuration["LdapSettings:Host"];
        var ldapPortString = _configuration["LdapSettings:Port"];
        var baseDn = _configuration["LdapSettings:BaseDn"];
        var profileAttribute = _configuration["LdapSettings:ProfileAttribute"];

        if (string.IsNullOrEmpty(ldapHost))
        {
            _logger.LogError("Configuration LDAP manquante (Host).");
            return Task.FromResult<string?>(null);
        }

        int ldapPort = int.TryParse(ldapPortString, out var p) ? p : 389;

        try
        {
            using (var connection = new LdapConnection())
            {
                // Connexion
                connection.Connect(ldapHost, ldapPort);

                // Bind (Authentification)
                // Construction du DN utilisateur 
                var domain = _configuration["LdapSettings:Domain"];

                // ==========================A supprimer après le test=====================
                //string bindDn;

                //if (string.IsNullOrEmpty(domain))
                //{
                //    // Mode LDAP Standard (pour le test forumsys)
                //    // On construit le DN complet car ce serveur l'exige pour le bind
                //    // Pour 'tesla', le DN est : uid=tesla,dc=example,dc=com
                //    bindDn = $"uid={username},{baseDn}";
                //}
                //else
                //{
                //    // Mode Active Directory (pour la prod)
                //    bindDn = $"{username}@{domain}";
                //}
                // ===========================Fin suppression===============================


                var userDn = string.IsNullOrEmpty(domain) ? username : $"{username}@{domain}";

                // Novell LDAP utilise la méthode Bind(string dn, string password)
                connection.Bind(userDn, password);

                if (connection.Bound)
                {
                    // Recherche
                    var searchFilter = $"(sAMAccountName={username})";
                    var searchResults = connection.Search(
                        baseDn,
                        LdapConnection.ScopeSub,
                        searchFilter,
                        new[] { profileAttribute },
                        false
                    );

                    if (searchResults.HasMore())
                    {
                        var entry = searchResults.Next();
                        var attributeSet = entry.GetAttributeSet();

                        var attribute = attributeSet.GetAttribute(profileAttribute);

                        if (attribute != null)
                        {
                            var profileCode = attribute.StringValue;
                            return Task.FromResult<string?>(profileCode);
                        }
                        else
                        {
                            _logger.LogWarning($"L'utilisateur {username} est authentifié mais l'attribut {profileAttribute} est introuvable.");
                            return Task.FromResult<string?>(null);
                        }
                    }
                }
            }
        }
        catch (LdapException ex)
        {
            _logger.LogWarning(ex, "Échec authentification LDAP pour {Username}. Code erreur: {Code}", username, ex.ResultCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur LDAP inattendue pour {Username}", username);
        }

        return Task.FromResult<string?>(null);
    }
}