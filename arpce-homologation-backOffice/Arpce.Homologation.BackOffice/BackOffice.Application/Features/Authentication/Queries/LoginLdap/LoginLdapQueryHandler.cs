using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Authentication.Queries.LoginLdap;

public class LoginLdapQueryHandler : IRequestHandler<LoginLdapQuery, AuthenticationResult>
{
    private readonly IApplicationDbContext _context;
    private readonly ILdapService _ldapService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginLdapQueryHandler(
        IApplicationDbContext context,
        ILdapService ldapService,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _context = context;
        _ldapService = ldapService;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthenticationResult> Handle(LoginLdapQuery request, CancellationToken cancellationToken)
    {
        // Authentification via LDAP
        var profileCode = await _ldapService.AuthenticateAndGetProfileCodeAsync(request.Username, request.Password);

        if (string.IsNullOrEmpty(profileCode))
        {
            throw new UnauthorizedAccessException("Identifiants incorrects ou profil non défini dans l'AD.");
        }

        // Cherche le profil localement
        var profil = await _context.AdminProfils
            .FirstOrDefaultAsync(p => p.Code == profileCode, cancellationToken);

        if (profil == null)
        {
            throw new UnauthorizedAccessException($"Le profil '{profileCode}' reçu de l'AD n'existe pas dans l'application.");
        }

        // Gère l'utilisateur local (Link)
        var adminUser = await _context.AdminUtilisateurs
            .FirstOrDefaultAsync(u => u.Compte == request.Username, cancellationToken);

        if (adminUser == null)
        {
            // Si l'utilisateur n'existe pas localement, on le crée.
            var defaultUserType = await _context.AdminUtilisateurTypes.FirstOrDefaultAsync(cancellationToken);
            if (defaultUserType == null)
            {
                throw new InvalidOperationException("Aucun type d'utilisateur par défaut n'est configuré dans la base.");
            }

            adminUser = new AdminUtilisateur
            {
                Id = Guid.NewGuid(),
                Compte = request.Username,
                Nom = request.Username, 
                IdProfil = profil.Id,
                IdUtilisateurType = defaultUserType.Id,
                DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Desactive = false,
                ChangementMotPasse = false 
            };
            _context.AdminUtilisateurs.Add(adminUser);
        }
        else
        {
            // Mise à jour du profil à chaque connexion pour refléter les changements de l'AD
            adminUser.IdProfil = profil.Id;
            adminUser.DerniereConnexion = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        await _context.SaveChangesAsync(cancellationToken);

        // On utilise le code du profil (venu du LDAP) comme nom de groupe pour SignalR
        string groupName = profileCode;

        // Génère le token JWT en incluant le profilCode et le groupName
        var token = _jwtTokenGenerator.GenerateToken(adminUser.Id, adminUser.Compte, profileCode, groupName);

        return new AuthenticationResult
        {
            Message = "Connexion LDAP réussie.",
            Token = token
        };
    }
}