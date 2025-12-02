using FrontOffice.Application.Common.Interfaces;
using MediatR;

namespace FrontOffice.Application.Features.Authentication.Commands.ConfirmAccount;

/// <summary>
/// Gère la validation du code OTP envoyé par e-mail.
/// Fait passer le compte du Niveau 0 (Inscrit) au Niveau 1 (En attente ARPCE).
/// </summary>
public class ConfirmAccountCommandHandler : IRequestHandler<ConfirmAccountCommand, AuthenticationResult>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ConfirmAccountCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<AuthenticationResult> Handle(ConfirmAccountCommand request, CancellationToken cancellationToken)
    {
        // Récupére l'ID de l'utilisateur depuis le token de VÉRIFICATION (celui reçu à l'inscription)
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("Le token de vérification est invalide, manquant ou a expiré.");
        }

        // Récupére le client en base de données
        var client = await _context.Clients.FindAsync(new object[] { userId.Value }, cancellationToken);

        if (client == null)
        {
            throw new InvalidOperationException("Compte utilisateur introuvable.");
        }

        // Vérifications Métier

        // Si le niveau est déjà >= 1, c'est que l'OTP a déjà été validé.
        if (client.NiveauValidation >= 1 || client.IsVerified)
        {
            // On considère cela comme une erreur métier pour informer l'utilisateur.
            throw new InvalidOperationException("Ce compte a déjà été vérifié par e-mail.");
        }

        // Vérification de l'expiration du code
        if (client.VerificationTokenExpiry < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Le code de vérification a expiré. Veuillez redemander une inscription pour obtenir un nouveau code.");
        }

        // Vérification de la validité du code
        // On utilise une comparaison insensible à la casse et sans espaces pour être user-friendly
        if (string.IsNullOrWhiteSpace(request.Code) || client.VerificationCode?.Trim() != request.Code.Trim())
        {
            throw new InvalidOperationException("Le code de vérification est incorrect.");
        }

        // Mise à jour du statut du compte
        // Passage au Niveau 1 : E-mail validé, mais en attente de validation administrative.
        client.NiveauValidation = 1;
        client.IsVerified = true;

        // Sécurité : On efface le code pour qu'il ne puisse plus être utilisé
        client.VerificationCode = null;
        client.VerificationTokenExpiry = null;

        await _context.SaveChangesAsync(cancellationToken);

        // Retourne le résultat
        // IMPORTANT : Token est vide car l'utilisateur n'a pas le droit de se connecter tant que l'ARPCE n'a pas validé (Niveau 2).
        return new AuthenticationResult
        {
            Message = "Votre e-mail a été vérifié avec succès. Votre compte est maintenant en attente de validation administrative par l'ARPCE. Vous recevrez un e-mail de confirmation dès que votre compte sera activé.",
            Token = string.Empty
        };
    }
}