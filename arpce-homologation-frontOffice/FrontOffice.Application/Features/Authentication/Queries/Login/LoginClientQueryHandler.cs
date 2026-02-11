using FrontOffice.Application.Common.Exceptions;
using FrontOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FrontOffice.Application.Features.Authentication.Queries.Login;

/// <summary>
/// Gère la tentative de connexion d'un client.
/// Vérifie les identifiants et le niveau de validation du compte (0, 1 ou 2).
/// </summary>
public class LoginClientQueryHandler : IRequestHandler<LoginClientQuery, AuthenticationResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;

    public LoginClientQueryHandler(
        IApplicationDbContext context,
        IJwtTokenGenerator jwtTokenGenerator,
        IPasswordHasher passwordHasher)
    {
        _context = context;
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthenticationResult> Handle(LoginClientQuery request, CancellationToken cancellationToken)
    {
        // Étape 1 : Rechercher le client par email
        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.Email == request.Email, cancellationToken);

        // Étape 2 : Valider l'existence et le mot de passe
        if (client is null || string.IsNullOrEmpty(client.MotPasse) || !_passwordHasher.Verify(request.Password, client.MotPasse))
        {
            // Message générique pour la sécurité
            throw new UnauthorizedAccessException("L'adresse e-mail ou le mot de passe est incorrect.");
        }

        // Vérifie si le compte est désactivé administrativement
        if (client.Desactive == 1)
        {
            throw new UnauthorizedAccessException("Ce compte client a été désactivé par l'administration.");
        }

        // Vérifie le Niveau de Validation (Workflow d'inscription)

        // Niveau 0 : Inscrit mais OTP non validé
        //if (client.NiveauValidation == 0)
        //{
        //    throw new UnauthorizedAccessException("Votre adresse e-mail n'a pas encore été vérifiée. Veuillez utiliser le code reçu lors de l'inscription.");
        //}

        // Niveau 1 : OTP validé mais en attente validation ARPCE
        // L'utilisateur doit attendre que l'admin passe le niveau à 2.
        if (client.NiveauValidation == 1)
        {
            throw new AccountPendingValidationException("Votre compte est en cours de validation par l'administration ARPCE (Niveau 1). Vous recevrez une notification dès qu'il sera actif.");
        }

        // Si on arrive ici, NiveauValidation >= 2 (Validé ARPCE)

        // Générer le token de connexion
        var token = _jwtTokenGenerator.GenerateToken(client.Id, client.Email);

        return new AuthenticationResult
        {
            Message = "Connexion réussie.",
            Token = token
        };
    }
}