using BackOffice.Application.Common.Interfaces;
using MediatR;

namespace BackOffice.Application.Features.Dossiers.Commands.DeleteDossier;

public class DeleteDossierCommandHandler : IRequestHandler<DeleteDossierCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public DeleteDossierCommandHandler(IApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<bool> Handle(DeleteDossierCommand request, CancellationToken cancellationToken)
    {
        var dossier = await _context.Dossiers.FindAsync(new object[] { request.Id }, cancellationToken);

        if (dossier == null)
        {
            throw new Exception($"Le dossier avec l'ID '{request.Id}' est introuvable.");
        }

        // Suppression physique du dossier.
        // EF Core gérera la suppression en cascade des enfants (Demandes, etc.) 
        // grâce à .OnDelete(DeleteBehavior.Cascade) configuré dans DossierConfiguration.
        _context.Dossiers.Remove(dossier);

        await _context.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync(
    page: "Gestion des Dossiers",
    libelle: $"Le dossier '{dossier.Numero}' (ID: {dossier.Id}) a été supprimé.",
    eventTypeCode: "SUPPRESSION",
    dossierId: dossier.Id);

        return true;
    }
}