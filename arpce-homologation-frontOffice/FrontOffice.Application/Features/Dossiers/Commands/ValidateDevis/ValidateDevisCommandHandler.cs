using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FrontOffice.Application.Features.Dossiers.Commands.ValidateDevis;

public class ValidateDevisCommandHandler : IRequestHandler<ValidateDevisCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService; 

    public ValidateDevisCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        INotificationService notificationService) 
    {
        _context = context;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
    }

    public async Task<bool> Handle(ValidateDevisCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue) throw new UnauthorizedAccessException("Utilisateur non authentifié.");

        var dossier = await _context.Dossiers
            .Include(d => d.Statut)
            .FirstOrDefaultAsync(d => d.Id == request.DossierId && d.IdClient == userId.Value, cancellationToken);

        if (dossier == null) throw new Exception($"Dossier introuvable ou non autorisé.");

        if (dossier.Statut?.Code != StatutDossierEnum.DevisEmit.ToString())
        {
            throw new InvalidOperationException($"Le devis de ce dossier n'est pas en attente de validation. Statut actuel : '{dossier.Statut?.Libelle}'.");
        }

        var nouveauStatut = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == StatutDossierEnum.DevisValide.ToString(), cancellationToken);
        if (nouveauStatut == null) throw new Exception("Statut 'DevisValide' introuvable.");

        dossier.IdStatut = nouveauStatut.Id;
        await _context.SaveChangesAsync(cancellationToken);

        await _notificationService.SendToGroupAsync(
            groupName: "DAFC", 
            title: "Devis Validé par le Client",
            message: $"Le client a accepté le devis pour le dossier {dossier.Numero}. Le paiement peut être initié.",
            type: "E",
            targetUrl: $"/dossiers/{dossier.Id}"
        );

        return true;
    }
}