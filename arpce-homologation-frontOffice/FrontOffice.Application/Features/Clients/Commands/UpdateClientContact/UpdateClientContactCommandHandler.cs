using FrontOffice.Application.Common.DTOs;
using FrontOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FrontOffice.Application.Features.Clients.Commands.UpdateClientContact;

public class UpdateClientContactCommandHandler : IRequestHandler<UpdateClientContactCommand, ClientContactDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService; 

    public UpdateClientContactCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        INotificationService notificationService) 
    {
        _context = context;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
    }

    public async Task<ClientContactDto> Handle(UpdateClientContactCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        if (!currentUserId.HasValue) throw new UnauthorizedAccessException("Non authentifié.");
        if (request.ClientId != currentUserId.Value) throw new UnauthorizedAccessException("Action non autorisée.");

        var clientToUpdate = await _context.Clients.FirstOrDefaultAsync(c => c.Id == request.ClientId, cancellationToken);
        if (clientToUpdate == null) throw new InvalidOperationException("Client introuvable.");

        bool hasChanges = false;
        if (request.ContactNom != null) { clientToUpdate.ContactNom = request.ContactNom; hasChanges = true; }
        if (request.ContactTelephone != null) { clientToUpdate.ContactTelephone = request.ContactTelephone; hasChanges = true; }
        if (request.ContactFonction != null) { clientToUpdate.ContactFonction = request.ContactFonction; hasChanges = true; }

        if (hasChanges)
        {
            await _context.SaveChangesAsync(cancellationToken);

            await _notificationService.SendToGroupAsync(
                groupName: "ADMIN",
                title: "Mise à Jour Client",
                message: $"Le client '{clientToUpdate.RaisonSociale}' a mis à jour ses informations de contact.",
                type: "Info"
            );
        }

        return new ClientContactDto
        {
            Id = clientToUpdate.Id,
            ContactNom = clientToUpdate.ContactNom,
            ContactTelephone = clientToUpdate.ContactTelephone,
            ContactFonction = clientToUpdate.ContactFonction
        };
    }
}