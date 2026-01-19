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
            // On utilise FirstOrDefaultAsync pour une recherche plus directe
            var demande = await _context.Demandes
                .Include(d => d.Dossier)
                .FirstOrDefaultAsync(d => d.Id == request.IdDemande, cancellationToken);

            if (demande == null)
            {
                _logger.LogError("Tentative d'upload de certificat pour une demande inexistante : {DemandeId}", request.IdDemande);
                throw new Exception("Demande introuvable.");
            }

            var file = request.CertificatFile;
            if (file == null || file.Length == 0)
                throw new InvalidOperationException("Le fichier du certificat est manquant.");

            byte[] fileData;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream, cancellationToken);
                fileData = memoryStream.ToArray();
            }

            // On vérifie si une attestation existe déjà pour la mettre à jour
            var attestation = await _context.Attestations.FirstOrDefaultAsync(a => a.IdDemande == demande.Id, cancellationToken);

            if (attestation != null)
            {
                // Mise à jour de l'attestation existante
                attestation.DateDelivrance = new DateTimeOffset(request.DateDelivrance).ToUnixTimeMilliseconds();
                attestation.DateExpiration = new DateTimeOffset(request.DateExpiration).ToUnixTimeMilliseconds();
                attestation.Donnees = fileData;
                attestation.Extension = Path.GetExtension(file.FileName)?.TrimStart('.').ToLowerInvariant() ?? "pdf";

                _context.Attestations.Update(attestation);
                _logger.LogInformation("Attestation existante {AttestationId} mise à jour pour la demande {DemandeId}", attestation.Id, demande.Id);
            }
            else
            {
                // Création d'une nouvelle attestation
                attestation = new Attestation
                {
                    Id = Guid.NewGuid(),
                    IdDemande = demande.Id,
                    DateDelivrance = new DateTimeOffset(request.DateDelivrance).ToUnixTimeMilliseconds(),
                    DateExpiration = new DateTimeOffset(request.DateExpiration).ToUnixTimeMilliseconds(),
                    Donnees = fileData,
                    Extension = Path.GetExtension(file.FileName)?.TrimStart('.').ToLowerInvariant() ?? "pdf"
                };
                _context.Attestations.Add(attestation);
                _logger.LogInformation("Nouvelle attestation créée pour la demande {DemandeId}", demande.Id);
            }

            // Met à jour le statut du dossier parent à "Attestation signée"
            var statutSigne = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == "DossierSigner", cancellationToken);
            if (statutSigne != null)
            {
                demande.Dossier.IdStatut = statutSigne.Id;
            }
            else
            {
                _logger.LogWarning("Le statut 'DossierSigner' est introuvable. Le statut du dossier n'a pas été mis à jour.");
            }

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            await _auditService.LogAsync("Gestion des Attestations", $"Certificat uploadé pour '{demande.Equipement}'.", "UPLOAD", demande.IdDossier);

            await _notificationService.SendToGroupAsync(
                profilCode: "CERTIFICATS",
                title: "Attestation Disponible",
                message: $"L'attestation pour le dossier {demande.Dossier.Numero} est signée et disponible.",
                type: "E",
                targetUrl: $"/dossiers/{demande.IdDossier}",
                entityId: demande.IdDossier.ToString()
            );

            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Échec de l'upload du certificat pour la demande {DemandeId}", request.IdDemande);
            throw;
        }
    }
}