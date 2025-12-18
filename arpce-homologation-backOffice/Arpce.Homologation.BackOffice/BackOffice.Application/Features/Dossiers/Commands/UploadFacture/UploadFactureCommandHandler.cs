using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Dossiers.Commands.UploadFacture;

public class UploadFactureCommandHandler : IRequestHandler<UploadFactureCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageProvider _fileStorageProvider;
    private readonly IAuditService _auditService;
    private readonly INotificationService _notificationService; 

    public UploadFactureCommandHandler(
        IApplicationDbContext context,
        IFileStorageProvider fileStorageProvider,
        IAuditService auditService,
        INotificationService notificationService)
    {
        _context = context;
        _fileStorageProvider = fileStorageProvider;
        _auditService = auditService;
        _notificationService = notificationService;
    }

    public async Task<bool> Handle(UploadFactureCommand request, CancellationToken cancellationToken)
    {
        var dossier = await _context.Dossiers.FindAsync(new object[] { request.DossierId }, cancellationToken);
        if (dossier == null) throw new Exception($"Dossier introuvable.");

        var file = request.FactureFile;
        if (file == null || file.Length == 0) throw new InvalidOperationException("Fichier manquant.");
        if (Path.GetExtension(file.FileName).ToLowerInvariant() != ".pdf") throw new InvalidOperationException("PDF requis.");

        await _fileStorageProvider.ImportDocumentDossierAsync(
            file: file,
            nom: $"Facture_{dossier.Numero}",
            type: 2,
            dossierId: dossier.Id
        );

        var codeStatut = "EnPaiement"; 
        var nouveauStatut = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == codeStatut, cancellationToken);

        if (nouveauStatut == null)
            nouveauStatut = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == "DevisPaiement", cancellationToken);

        if (nouveauStatut != null) dossier.IdStatut = nouveauStatut.Id;

        await _context.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync("Gestion des Dossiers", $"Facture téléversée pour '{dossier.Numero}'.", "UPLOAD", dossier.Id);

        await _notificationService.SendToGroupAsync(
            profilCode: "DOSSIERS",
            title: "Facture Émise",
            message: $"La facture pour le dossier {dossier.Numero} est disponible. En attente de paiement.",
            type: "V",
            targetUrl: $"/dossiers/{dossier.Id}",
            entityId: dossier.Id.ToString()
        );

        return true;
    }
}