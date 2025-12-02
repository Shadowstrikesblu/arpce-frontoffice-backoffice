using BackOffice.Application.Common.Interfaces;
using MediatR;

namespace BackOffice.Application.Features.Demandes.Commands.SetHomologable;

public class SetEquipementHomologableCommandHandler : IRequestHandler<SetEquipementHomologableCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public SetEquipementHomologableCommandHandler(IApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<bool> Handle(SetEquipementHomologableCommand request, CancellationToken cancellationToken)
    {
        // Récupérer l'équipement
        var demande = await _context.Demandes.FindAsync(new object[] { request.EquipementId }, cancellationToken);

        if (demande == null)
        {
            throw new Exception($"L'équipement (demande) avec l'ID '{request.EquipementId}' est introuvable.");
        }

        // Mise à jour de la propriété
        demande.EstHomologable = request.Homologable;

        // Sauvegarde
        await _context.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync(
    page: "Instruction Équipement",
    libelle: $"L'équipement ID '{demande.Id}' a été marqué comme {(request.Homologable ? "Homologable" : "Non Homologable")}.",
    eventTypeCode: "QUALIFICATION",
    dossierId: demande.IdDossier);

        return true;
    }
}