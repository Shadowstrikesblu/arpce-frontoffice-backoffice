using MediatR;

namespace FrontOffice.Application.Features.Authentication.Commands.ConfirmAccount;

/// <summary>
/// Commande pour confirmer le compte d'un utilisateur à l'aide d'un code de vérification.
/// Cette commande doit être exécutée avec un token de vérification temporaire.
/// </summary>
public class ConfirmAccountCommand : IRequest<AuthenticationResult> 
{
    /// <summary>
    /// Le code de vérification à 6 chiffres reçu par e-mail par l'utilisateur.
    /// </summary>
    public string Code { get; set; } = string.Empty;
}