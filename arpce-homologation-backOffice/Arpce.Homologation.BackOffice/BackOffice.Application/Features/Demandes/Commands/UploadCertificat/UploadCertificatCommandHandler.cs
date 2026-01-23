using BackOffice.Application.Common.Interfaces;
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
            // Récupération de l'attestation avec sa demande et son dossier
            var attestation = await _context.Attestations
                .Include(a => a.Demande)
                    .ThenInclude(d => d.Dossier)
                .FirstOrDefaultAsync(a => a.Id == request.AttestationId, cancellationToken);

            if (attestation == null)
            {
                _logger.LogWarning("Attestation {AttestationId} introuvable.", request.AttestationId);
                return false;
            }

            var dossier = attestation.Demande.Dossier;

            // Validation du fichier
            if (request.CertificatFile == null || request.CertificatFile.Length == 0)
            {
                _logger.LogWarning("Fichier manquant pour l'attestation {AttestationId}.", request.AttestationId);
                return false;
            }

            // Lecture du fichier en mémoire
            byte[] fileData;
            using (var memoryStream = new MemoryStream())
            {
                await request.CertificatFile.CopyToAsync(memoryStream, cancellationToken);
                fileData = memoryStream.ToArray();
            }

            // Mise à jour de l'attestation
            attestation.Donnees = fileData;
            attestation.Extension = Path.GetExtension(request.CertificatFile.FileName)?.TrimStart('.').ToLowerInvariant() ?? "pdf";
            attestation.DateDelivrance = new DateTimeOffset(request.DateDelivrance).ToUnixTimeMilliseconds();
            attestation.DateExpiration = new DateTimeOffset(request.DateExpiration).ToUnixTimeMilliseconds();

            _context.Attestations.Update(attestation);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Attestation {AttestationId} uploadée avec succès.", request.AttestationId);

            // Comptage EXACT selon la règle métier
            var totalDemandesDuDossier = await _context.Demandes
                .CountAsync(d => d.IdDossier == dossier.Id, cancellationToken);

            var attestationsUploadees = await _context.Attestations
                .CountAsync(a =>
                    a.Demande.IdDossier == dossier.Id &&
                    a.Donnees != null &&
                    a.Donnees.Length > 0,
                    cancellationToken);

            // Changement de statut UNIQUEMENT si toutes les attestations sont uploadées
            if (totalDemandesDuDossier > 0 && attestationsUploadees == totalDemandesDuDossier)
            {
                _logger.LogInformation(
                    "Toutes les attestations du dossier {DossierId} sont uploadées. Mise à jour du statut.",
                    dossier.Id
                );

                var statutSigne = await _context.Statuts
                    .FirstOrDefaultAsync(s => s.Code == "DossierSigner", cancellationToken);

                if (statutSigne != null && dossier.IdStatut != statutSigne.Id)
                {
                    dossier.IdStatut = statutSigne.Id;
                    _context.Dossiers.Update(dossier);
                }
            }
            else
            {
                _logger.LogInformation(
                    "Attestations uploadées : {Uploadees}/{Total} pour le dossier {DossierId}.",
                    attestationsUploadees,
                    totalDemandesDuDossier,
                    dossier.Id
                );
            }

            // Sauvegarde finale
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            // Audit
            await _auditService.LogAsync(
                "Gestion Attestations",
                $"Certificat uploadé pour la demande '{attestation.Demande.Equipement}'.",
                "UPLOAD",
                dossier.Id
            );

            // Notifications
            var message = $"Le certificat pour l'équipement '{attestation.Demande.Equipement}' du dossier {dossier.Numero} est disponible.";

            await _notificationService.SendToGroupAsync(
                "DRSCE",
                "Certificat Signé",
                message,
                "E",
                $"/dossiers/{dossier.Id}",
                dossier.Id.ToString()
            );

            await _notificationService.SendToGroupAsync(
                "DAJI",
                "Certificat Signé",
                message,
                "V",
                $"/dossiers/{dossier.Id}",
                dossier.Id.ToString()
            );

            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Erreur lors de l'upload du certificat pour l'attestation {AttestationId}.", request.AttestationId);
            return false;
        }
    }
}
