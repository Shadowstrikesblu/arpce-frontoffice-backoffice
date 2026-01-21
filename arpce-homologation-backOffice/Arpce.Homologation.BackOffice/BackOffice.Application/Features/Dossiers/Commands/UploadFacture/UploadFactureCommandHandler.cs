using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BackOffice.Application.Features.Dossiers.Commands.UploadFacture;

public class UploadFactureCommandHandler : IRequestHandler<UploadFactureCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageProvider _fileStorageProvider;
    private readonly IAuditService _auditService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<UploadFactureCommandHandler> _logger;

    public UploadFactureCommandHandler(
        IApplicationDbContext context,
        IFileStorageProvider fileStorageProvider,
        IAuditService auditService,
        INotificationService notificationService,
        ILogger<UploadFactureCommandHandler> logger)
    {
        _context = context;
        _fileStorageProvider = fileStorageProvider;
        _auditService = auditService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<bool> Handle(UploadFactureCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
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

            if (nouveauStatut != null)
            {
                dossier.IdStatut = nouveauStatut.Id;
            }
            else
            {
                _logger.LogError("Statut 'EnPaiement' introuvable. Le statut du dossier n'a pas été mis à jour.");
            }

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            await _auditService.LogAsync("Gestion des Dossiers", $"Facture téléversée pour '{dossier.Numero}'.", "UPLOAD", dossier.Id);

            await _notificationService.SendToGroupAsync(
                profilCode: "DAFC",
                title: "Facture Émise",
                message: $"La facture pour le dossier {dossier.Numero} est disponible. En attente de paiement.",
                type: "V",
                targetUrl: $"/dossiers/{dossier.Id}",
                entityId: dossier.Id.ToString()
            );

            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Échec de l'upload de la facture pour le dossier {DossierId}", request.DossierId);
            throw;
        }
    }
}