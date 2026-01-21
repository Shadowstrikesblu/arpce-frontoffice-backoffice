using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Demandes.Commands.UpdateEquipement;

public class UpdateEquipementCommandHandler : IRequestHandler<UpdateEquipementCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;
    private readonly INotificationService _notificationService;

    public UpdateEquipementCommandHandler(
        IApplicationDbContext context,
        IAuditService auditService,
        INotificationService notificationService)
    {
        _context = context;
        _auditService = auditService;
        _notificationService = notificationService;
    }

    public async Task<bool> Handle(UpdateEquipementCommand request, CancellationToken cancellationToken)
    {
        var demande = await _context.Demandes
            .Include(d => d.Dossier)
            .FirstOrDefaultAsync(d => d.Id == request.EquipementId, cancellationToken);

        if (demande == null) throw new Exception("Équipement introuvable.");

        if (request.Equipement != null) demande.Equipement = request.Equipement;
        if (request.Modele != null) demande.Modele = request.Modele;
        if (request.Marque != null) demande.Marque = request.Marque;
        if (request.Fabricant != null) demande.Fabricant = request.Fabricant;
        if (request.Type != null) demande.Type = request.Type;
        if (request.Description != null) demande.Description = request.Description;
        if (request.QuantiteEquipements.HasValue) demande.QuantiteEquipements = request.QuantiteEquipements;

        await _context.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync(
            page: "Modification Équipement",
            libelle: $"L'équipement '{demande.Equipement}' (ID: {demande.Id}) a été modifié.",
            eventTypeCode: "MODIFICATION",
            dossierId: demande.IdDossier
        );

        await _notificationService.SendToGroupAsync(
            profilCode: "DRSCE",
            title: "Mise à Jour Équipement",
            message: $"Les informations de l'équipement '{demande.Equipement}' (dossier {demande.Dossier.Numero}) ont été mises à jour.",
            type: "V"
        );

        return true;
    }
}