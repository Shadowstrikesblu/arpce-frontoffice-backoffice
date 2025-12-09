using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Authentication.Queries.Login;

/// <summary>
/// Gère la logique de la requête de connexion d'un agent.
/// </summary>
public class LoginAgentQueryHandler : IRequestHandler<LoginAgentQuery, AuthenticationResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;

    /// <summary>
    /// Initialise une nouvelle instance du handler.
    /// </summary>
    public LoginAgentQueryHandler(
        IApplicationDbContext context,
        IJwtTokenGenerator jwtTokenGenerator,
        IPasswordHasher passwordHasher)
    {
        _context = context;
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = passwordHasher;
    }

    /// <summary>
    /// Exécute la logique de la requête de connexion.
    /// </summary>
    public async Task<AuthenticationResult> Handle(LoginAgentQuery request, CancellationToken cancellationToken)
    {
        // Recherche l'agent dans la base de données par son nom de compte.
        var agent = await _context.AdminUtilisateurs
            .FirstOrDefaultAsync(u => u.Compte == request.Compte, cancellationToken);

        // Valide l'existence de l'agent et la validité du mot de passe.
        if (agent is null || string.IsNullOrEmpty(agent.MotPasse) || !_passwordHasher.Verify(request.Password, agent.MotPasse))
        {
            // Leve une exception d'accès non autorisé. Le message est volontairement générique pour des raisons de sécurité.
            throw new UnauthorizedAccessException("Nom de compte ou mot de passe invalide.");
        }

        // Vérifie si le compte de l'agent est désactivé.
        if (agent.Desactive)
        {
            throw new UnauthorizedAccessException("Ce compte agent est désactivé.");
        }

        // Met à jour la date de dernière connexion 
        agent.DerniereConnexion = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); 
        await _context.SaveChangesAsync(cancellationToken);


        // Si l'authentification est réussie, génére un nouveau token JWT.
        var token = _jwtTokenGenerator.GenerateToken(agent.Id, agent.Compte);

        // Retourne le résultat avec un message de succès et le token.
        return new AuthenticationResult
        {
            Message = "Connexion réussie.",
            Token = token
        };
    }
}