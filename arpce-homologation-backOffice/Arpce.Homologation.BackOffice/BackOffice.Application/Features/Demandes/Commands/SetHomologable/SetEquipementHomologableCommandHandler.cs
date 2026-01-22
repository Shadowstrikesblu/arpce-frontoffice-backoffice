using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Demandes.Commands.SetHomologable;

public class SetEquipementHomologableCommandHandler : IRequestHandler<SetEquipementHomologableCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;
    private readonly INotificationService _notificationService;

    public SetEquipementHomologableCommandHandler(
        IApplicationDbContext context,
        IAuditService auditService,
        INotificationService notificationService)
    {
        _context = context;
        _auditService = auditService;
        _notificationService = notificationService;
    }

    public async Task<bool> Handle(SetEquipementHomologableCommand request, CancellationToken cancellationToken)
    {
        var demande = await _context.Demandes
            .Include(d => d.Dossier)
            .FirstOrDefaultAsync(d => d.Id == request.EquipementId, cancellationToken);

        if (demande == null) throw new Exception("Équipement introuvable.");

        demande.EstHomologable = request.Homologable;
        await _context.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync(
            page: "Instruction Équipement",
            libelle: $"L'équipement '{demande.Equipement}' a été marqué comme {(request.Homologable ? "Homologable" : "Non Homologable")}.",
            eventTypeCode: "QUALIFICATION",
            dossierId: demande.IdDossier);

        await _notificationService.SendToGroupAsync(
            profilCode: "DRSCE",
            title: "Qualification Équipement",
            message: $"L'équipement '{demande.Equipement}' (dossier {demande.Dossier.Numero}) a été qualifié.",
            type: "V"
        );

        return true;
    }
}