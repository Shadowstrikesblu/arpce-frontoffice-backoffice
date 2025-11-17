using FrontOffice.Application.Common.DTOs;
using FrontOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FrontOffice.Application.Features.Clients.Commands.UpdateClientContact;

/// <summary>
/// Gère la logique de la commande pour mettre à jour les informations de contact d'un client.
/// </summary>
public class UpdateClientContactCommandHandler : IRequestHandler<UpdateClientContactCommand, ClientContactDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    /// Initialise une nouvelle instance du handler.
    /// </summary>
    /// <param name="context">Le contexte de la base de données.</param>
    /// <param name="currentUserService">Le service pour obtenir l'utilisateur connecté.</param>
    public UpdateClientContactCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Exécute la logique de mise à jour des informations de contact du client.
    /// </summary>
    /// <param name="request">La commande contenant les nouvelles informations de contact.</param>
    /// <param name="cancellationToken">Le token d'annulation.</param>
    /// <returns>Un DTO contenant les informations de contact mises à jour.</returns>
    /// <exception cref="UnauthorizedAccessException">Levée si l'utilisateur n'est pas authentifié ou essaie de modifier un autre profil.</exception>
    /// <exception cref="InvalidOperationException">Levée si le client spécifié n'est pas trouvé.</exception>
    public async Task<ClientContactDto> Handle(UpdateClientContactCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        // --- Vérifie l'authentification ---
        if (!currentUserId.HasValue)
        {
            throw new UnauthorizedAccessException("Utilisateur non authentifié.");
        }

        // --- Vérifie que l'utilisateur modifie son propre profil ---
        // Le ClientId de la commande (venant de la route) doit correspondre à l'ID de l'utilisateur dans le token JWT.
        if (request.ClientId != currentUserId.Value)
        {
            throw new UnauthorizedAccessException("Vous n'êtes pas autorisé à modifier les informations de cet utilisateur.");
        }

        // --- Récupération du client à mettre à jour ---
        var clientToUpdate = await _context.Clients
            .FirstOrDefaultAsync(c => c.Id == request.ClientId, cancellationToken);

        if (clientToUpdate == null)
        {
            // Cette exception ne devrait normalement jamais être atteinte si l'ID vient du token,
            // mais c'est une sécurité en cas de désynchronisation de la base de données.
            throw new InvalidOperationException($"Le client avec l'ID '{request.ClientId}' est introuvable.");
        }

        // --- Mise à jour des champs ---
        bool hasChanges = false;

        if (request.ContactNom != null)
        {
            clientToUpdate.ContactNom = request.ContactNom;
            hasChanges = true;
        }

        if (request.ContactTelephone != null)
        {
            clientToUpdate.ContactTelephone = request.ContactTelephone;
            hasChanges = true;
        }

        if (request.ContactFonction != null)
        {
            clientToUpdate.ContactFonction = request.ContactFonction;
            hasChanges = true;
        }

        // --- Sauvegarde des changements ---
        if (hasChanges)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        // --- Préparation et retour de la réponse ---
        return new ClientContactDto
        {
            Id = clientToUpdate.Id,
            ContactNom = clientToUpdate.ContactNom,
            ContactTelephone = clientToUpdate.ContactTelephone,
            ContactFonction = clientToUpdate.ContactFonction
        };
    }
}