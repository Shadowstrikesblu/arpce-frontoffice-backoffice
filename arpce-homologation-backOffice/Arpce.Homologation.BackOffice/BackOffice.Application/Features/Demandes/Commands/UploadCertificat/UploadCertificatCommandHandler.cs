using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace BackOffice.Application.Features.Demandes.Commands.UploadCertificat;

public class UploadCertificatCommandHandler : IRequestHandler<UploadCertificatCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<UploadCertificatCommandHandler> _logger;

    public UploadCertificatCommandHandler(
        IApplicationDbContext context,
        IAuditService auditService,
        INotificationService notificationService,
        ILogger<UploadCertificatCommandHandler> logger)
    {
        _context = context;
        _auditService = auditService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<bool> Handle(UploadCertificatCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // Trouve l'attestation par son ID et inclure les parents nécessaires
            var attestationToUpdate = await _context.Attestations
                .Include(a => a.Demande)
                    .ThenInclude(dem => dem.Dossier)
                .FirstOrDefaultAsync(a => a.Id == request.AttestationId, cancellationToken);

            if (attestationToUpdate == null)
            {
                throw new Exception($"Attestation avec l'ID '{request.AttestationId}' introuvable.");
            }

            var dossier = attestationToUpdate.Demande.Dossier;

            // Traite le fichier uploadé
            var file = request.CertificatFile;
            if (file == null || file.Length == 0)
            {
                throw new InvalidOperationException("Le fichier du certificat est manquant.");
            }

            byte[] fileData;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream, cancellationToken);
                fileData = memoryStream.ToArray();
            }

            // Met à jour l'attestation avec le PDF et la date du jour
            attestationToUpdate.DateDelivrance = new DateTimeOffset(request.DateDelivrance).ToUnixTimeMilliseconds();
            attestationToUpdate.DateExpiration = new DateTimeOffset(request.DateExpiration).ToUnixTimeMilliseconds();
            attestationToUpdate.Donnees = fileData;
            attestationToUpdate.Extension = Path.GetExtension(file.FileName)?.TrimStart('.').ToLowerInvariant() ?? "pdf";

            // On met à jour l'entité dans le contexte
            _context.Attestations.Update(attestationToUpdate);

            // On sauvegarde une première fois pour que le Count suivant soit correct
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Attestation {AttestationId} mise à jour avec le contenu du fichier.", request.AttestationId);

            // 4. Vérifier si TOUTES les attestations du DOSSIER sont désormais complètes
            var allDemandeIdsInDossier = await _context.Demandes
                .Where(d => d.IdDossier == dossier.Id)
                .Select(d => d.Id)
                .ToListAsync(cancellationToken);

            var totalAttestationsInDossier = allDemandeIdsInDossier.Count;

            var completedAttestationsCount = await _context.Attestations
                .CountAsync(a => allDemandeIdsInDossier.Contains(a.IdDemande) && a.Donnees != null && a.Donnees.Length > 1, cancellationToken);

            if (completedAttestationsCount == totalAttestationsInDossier)
            {
                _logger.LogInformation("Toutes les {Count} attestations du dossier {DossierId} sont complètes. Changement de statut.", totalAttestationsInDossier, dossier.Id);
                var statutSigne = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == "DossierSigner", cancellationToken);
                if (statutSigne != null)
                {
                    dossier.IdStatut = statutSigne.Id;
                    _context.Dossiers.Update(dossier);
                }
            }
            else
            {
                _logger.LogInformation("{Completed}/{Total} attestations complétées pour le dossier {DossierId}. Statut inchangé.", completedAttestationsCount, totalAttestationsInDossier, dossier.Id);
            }

            // Sauvegarde finale (pour le changement de statut éventuel)
            await _context.SaveChangesAsync(cancellationToken);

            // Valider la transaction
            await transaction.CommitAsync(cancellationToken);

            await _auditService.LogAsync("Gestion Attestations", $"Certificat uploadé pour l'équipement '{attestationToUpdate.Demande.Equipement}'.", "UPLOAD", dossier.Id);

            var message = $"Le certificat pour l'équipement '{attestationToUpdate.Demande.Equipement}' du dossier {dossier.Numero} est disponible.";
            await _notificationService.SendToGroupAsync("DRSCE", "Certificat Signé", message, "E", $"/dossiers/{dossier.Id}", dossier.Id.ToString());
            await _notificationService.SendToGroupAsync("DAJI", "Certificat Signé", message, "V", $"/dossiers/{dossier.Id}", dossier.Id.ToString());

            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Échec de l'upload du certificat pour l'attestation {AttestationId}", request.AttestationId);
            throw;
        }
    }
}