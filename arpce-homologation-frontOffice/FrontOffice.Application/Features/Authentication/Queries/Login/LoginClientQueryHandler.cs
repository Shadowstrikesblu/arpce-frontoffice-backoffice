using FrontOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FrontOffice.Application.Features.Authentication.Queries.Login;

/// <summary>
/// Gère la logique de la requête de connexion d'un client.
/// </summary>
public class LoginClientQueryHandler : IRequestHandler<LoginClientQuery, AuthenticationResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;

    /// <summary>
    /// Initialise une nouvelle instance du handler.
    /// </summary>
    /// <param name="context">Le contexte de la base de données.</param>
    /// <param name="jwtTokenGenerator">Le service de génération de token JWT.</param>
    /// <param name="passwordHasher">Le service de vérification de mot de passe.</param>
    public LoginClientQueryHandler(
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
    /// <param name="request">La requête de connexion contenant l'email et le mot de passe.</param>
    /// <param name="cancellationToken">Le token d'annulation.</param>
    /// <returns>Un résultat d'authentification contenant un message de succès et un token JWT.</returns>
    /// <exception cref="UnauthorizedAccessException">Levée si les informations d'identification sont invalides.</exception>
    public async Task<AuthenticationResult> Handle(LoginClientQuery request, CancellationToken cancellationToken)
    {
        // Recherche le client dans la base de données par son email
        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.Email == request.Email, cancellationToken);

        // Vérifie si le client existe et si son mot de passe est défini
        if (client is null || string.IsNullOrEmpty(client.MotPasse))
        {
            // Retourne une erreur d'autorisation générique pour des raisons de sécurité
            throw new UnauthorizedAccessException("Email ou mot de passe invalide.");
        }

        // Vérifie si le client est désactivé
        if (client.Desactive == 1)
        {
            throw new UnauthorizedAccessException("Ce compte client est désactivé.");
        }

        // Vérifie si le mot de passe fourni correspond au hash stocké
        var isPasswordValid = _passwordHasher.Verify(request.Password, client.MotPasse);

        if (!isPasswordValid)
        {
            // Retourne la même erreur générique pour ne pas indiquer quelle partie est incorrecte
            throw new UnauthorizedAccessException("Email ou mot de passe invalide.");
        }

        // Si l'authentification est réussie, génére un nouveau token JWT
        var token = _jwtTokenGenerator.GenerateToken(client.Id, client.Email);

        // Retourne le résultat avec un message de succès et le token
        return new AuthenticationResult
        {
            Message = "Connexion réussie.",
            Token = token
        };
    }
}