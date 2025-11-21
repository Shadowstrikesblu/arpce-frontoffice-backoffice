using FrontOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FrontOffice.Application.Features.Authentication.Queries.Login;

/// <summary>
/// Gère la logique de la requête de connexion d'un client.
/// Cette version inclut la vérification du statut d'activation du compte.
/// </summary>
public class LoginClientQueryHandler : IRequestHandler<LoginClientQuery, AuthenticationResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;

    /// <summary>
    /// Initialise une nouvelle instance du handler.
    /// </summary>
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
    /// <returns>Un résultat d'authentification contenant un message de succès et un token de connexion.</returns>
    /// <exception cref="UnauthorizedAccessException">Levée si les informations d'identification sont invalides, si le compte est désactivé ou non vérifié.</exception>
    public async Task<AuthenticationResult> Handle(LoginClientQuery request, CancellationToken cancellationToken)
    {
        // Recherche le client dans la base de données par son e-mail.
        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.Email == request.Email, cancellationToken);

        // Valide l'existence du client et la correspondance du mot de passe.
        if (client is null || string.IsNullOrEmpty(client.MotPasse) || !_passwordHasher.Verify(request.Password, client.MotPasse))
        {
            // Leve une exception avec un message générique pour la sécurité.
            throw new UnauthorizedAccessException("L'adresse e-mail ou le mot de passe est incorrect.");
        }

        // Vérifie si le compte client est désactivé par un administrateur.
        if (client.Desactive == 1)
        {
            throw new UnauthorizedAccessException("Ce compte client a été désactivé.");
        }

        if (!client.IsVerified)
        {
            // Si le compte n'est pas vérifié, on refuse la connexion et on donne une instruction claire.
            // L'utilisateur devra refaire le processus d'inscription pour recevoir un nouveau code.
            throw new UnauthorizedAccessException("Votre compte n'a pas encore été vérifié. Veuillez vérifier votre e-mail ou vous réinscrire pour recevoir un nouveau code de confirmation.");
        }

        // Si l'authentification est réussie, générer un nouveau token de connexion.
        var token = _jwtTokenGenerator.GenerateToken(client.Id, client.Email);

        //  Retourne le résultat avec un message de succès et le token.
        return new AuthenticationResult
        {
            Message = "Connexion réussie.",
            Token = token 
        };
    }
}