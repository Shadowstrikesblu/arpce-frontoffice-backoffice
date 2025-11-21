using FrontOffice.Application.Common.Interfaces;
using MediatR;

namespace FrontOffice.Application.Features.Authentication.Commands.ConfirmAccount;

/// <summary>
/// Gère la logique de la commande de confirmation de compte.
/// </summary>
public class ConfirmAccountCommandHandler : IRequestHandler<ConfirmAccountCommand, AuthenticationResult>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    /// <summary>
    /// Initialise une nouvelle instance du handler.
    /// </summary>
    public ConfirmAccountCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _context = context;
        _currentUserService = currentUserService;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    /// <summary>
    /// Exécute la logique de confirmation du compte.
    /// </summary>
    public async Task<AuthenticationResult> Handle(ConfirmAccountCommand request, CancellationToken cancellationToken)
    {
        // Récupére l'ID de l'utilisateur depuis le token de VÉRIFICATION.
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            // Cette exception sera levée si le token est manquant, invalide ou expiré.
            throw new UnauthorizedAccessException("Le token de vérification est invalide ou a expiré.");
        }

        // Récupére le client correspondant dans la base de données.
        var client = await _context.Clients.FindAsync(userId.Value);
        if (client == null)
        {
            throw new InvalidOperationException("Utilisateur introuvable. Veuillez réessayer de vous inscrire.");
        }

        // Valide le statut du compte et le code de vérification.
        if (client.IsVerified)
        {
            throw new InvalidOperationException("Ce compte a déjà été vérifié.");
        }

        if (client.VerificationTokenExpiry < DateTime.UtcNow)
        {
            // Le token/code a expiré. L'utilisateur doit redemander un code via l'endpoint d'inscription.
            throw new InvalidOperationException("Le code de vérification a expiré. Veuillez demander un nouveau code en tentant de vous réinscrire.");
        }

        if (client.VerificationCode != request.Code)
        {
            // Le code fourni est incorrect.
            throw new InvalidOperationException("Le code de vérification est incorrect.");
        }

        // Le code est correct, on active le compte.
        client.IsVerified = true;
        client.VerificationCode = null; 
        client.VerificationTokenExpiry = null; 

        await _context.SaveChangesAsync(cancellationToken);

        // Génére un vrai token de CONNEXION (valable 1h).
        var connectionToken = _jwtTokenGenerator.GenerateToken(client.Id, client.Email!);

        // Retourne le résultat avec le nouveau token de connexion.
        return new AuthenticationResult
        {
            Message = "Votre compte a été vérifié avec succès. Vous êtes maintenant connecté.",
            Token = connectionToken
        };
    }
}