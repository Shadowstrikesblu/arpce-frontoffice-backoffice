using BackOffice.Application.Common.Interfaces;
using MediatR;

namespace BackOffice.Application.Features.Demandes.Commands.UpdateEquipement;

public class UpdateEquipementCommandHandler : IRequestHandler<UpdateEquipementCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public UpdateEquipementCommandHandler(IApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<bool> Handle(UpdateEquipementCommand request, CancellationToken cancellationToken)
    {
        var demande = await _context.Demandes.FindAsync(new object[] { request.EquipementId }, cancellationToken);
        if (demande == null) throw new Exception("Équipement introuvable.");

        // Mise à jour partielle
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

        return true;
    }
}