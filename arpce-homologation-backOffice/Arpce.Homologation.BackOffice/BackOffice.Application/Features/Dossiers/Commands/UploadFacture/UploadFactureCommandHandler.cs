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
            // Récupération du dossier AVEC son statut 
            var dossier = await _context.Dossiers
                .Include(d => d.Statut)
                .FirstOrDefaultAsync(d => d.Id == request.DossierId, cancellationToken);

            if (dossier == null)
            {
                _logger.LogWarning("Tentative d'upload de facture pour un dossier inexistant : {DossierId}", request.DossierId);
                throw new Exception($"Dossier introuvable.");
            }

            if (dossier.Statut == null || dossier.Statut.Code != "DossierPayer")
            {
                _logger.LogWarning("Refus d'upload facture. Le dossier {DossierId} est au statut {Statut} au lieu de DossierPayer.", dossier.Id, dossier.Statut?.Code);

                throw new InvalidOperationException(
                    $"L'upload de la facture n'est autorisé que si le dossier est au statut 'Paiement effectué' (DossierPayer). " +
                    $"Statut actuel : '{dossier.Statut?.Libelle ?? "Inconnu"}'.");
            }

            // Validation du fichier
            var file = request.FactureFile;
            if (file == null || file.Length == 0)
            {
                throw new InvalidOperationException("Le fichier de la facture est manquant.");
            }

            if (Path.GetExtension(file.FileName).ToLowerInvariant() != ".pdf")
            {
                throw new InvalidOperationException("La facture doit être au format PDF.");
            }

            // Stockage du document
            await _fileStorageProvider.ImportDocumentDossierAsync(
                file: file,
                nom: $"Facture_{dossier.Numero}",
                type: 2, 
                dossierId: dossier.Id
            );


            // Sauvegarde
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            // Audit
            await _auditService.LogAsync(
                page: "Gestion des Dossiers",
                libelle: $"Facture finale téléversée pour '{dossier.Numero}'.",
                eventTypeCode: "UPLOAD",
                dossierId: dossier.Id);

            // Notification
            await _notificationService.SendToGroupAsync(
                profilCode: "DAFC",
                title: "Facture Disponible",
                message: $"La facture finale pour le dossier {dossier.Numero} a été téléversée.",
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