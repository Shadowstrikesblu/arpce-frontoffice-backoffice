using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Demandes.Commands.UploadCertificat;

public class UploadCertificatCommandHandler : IRequestHandler<UploadCertificatCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;
    private readonly INotificationService _notificationService; 

    public UploadCertificatCommandHandler(
        IApplicationDbContext context,
        IAuditService auditService,
        INotificationService notificationService)
    {
        _context = context;
        _auditService = auditService;
        _notificationService = notificationService;
    }

    public async Task<bool> Handle(UploadCertificatCommand request, CancellationToken cancellationToken)
    {
        var demande = await _context.Demandes
            .Include(d => d.Dossier)
            .FirstOrDefaultAsync(d => d.Id == request.DemandeId, cancellationToken);

        if (demande == null) throw new Exception("Demande introuvable.");

        var file = request.CertificatFile;
        byte[] fileData;
        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream, cancellationToken);
            fileData = memoryStream.ToArray();
        }

        var attestation = new Attestation
        {
            Id = Guid.NewGuid(),
            IdDemande = demande.Id,
            DateDelivrance = new DateTimeOffset(request.DateDelivrance).ToUnixTimeMilliseconds(),
            DateExpiration = new DateTimeOffset(request.DateExpiration).ToUnixTimeMilliseconds(),
            Donnees = fileData,
            Extension = Path.GetExtension(file.FileName).TrimStart('.')
        };

        _context.Attestations.Add(attestation);

        var statutSigne = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == "DossierSigner", cancellationToken);
        if (statutSigne != null) demande.Dossier.IdStatut = statutSigne.Id;

        await _context.SaveChangesAsync(cancellationToken);

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
}