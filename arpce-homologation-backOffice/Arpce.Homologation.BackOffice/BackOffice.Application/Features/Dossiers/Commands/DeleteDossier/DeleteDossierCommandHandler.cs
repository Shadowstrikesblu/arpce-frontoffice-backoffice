using BackOffice.Application.Common.Interfaces;
using MediatR;

namespace BackOffice.Application.Features.Dossiers.Commands.DeleteDossier;

public class DeleteDossierCommandHandler : IRequestHandler<DeleteDossierCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;
    private readonly INotificationService _notificationService;

    public DeleteDossierCommandHandler(IApplicationDbContext context, IAuditService auditService, INotificationService notificationService)
    {
        _context = context;
        _auditService = auditService;
        _notificationService = notificationService;
    }

    public async Task<bool> Handle(DeleteDossierCommand request, CancellationToken cancellationToken)
    {
        var dossier = await _context.Dossiers.FindAsync(new object[] { request.Id }, cancellationToken);
        if (dossier == null) throw new Exception("Dossier introuvable.");

        string numero = dossier.Numero;

        _context.Dossiers.Remove(dossier);
        await _context.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync("Gestion des Dossiers", $"Le dossier '{numero}' a été supprimé.", "SUPPRESSION", request.Id);

        await _notificationService.SendToGroupAsync(
            profilCode: "ADMIN",
            title: "Dossier Supprimé",
            message: $"Le dossier {numero} a été supprimé du système.",
            type: "E"
        );

        return true;
    }
}