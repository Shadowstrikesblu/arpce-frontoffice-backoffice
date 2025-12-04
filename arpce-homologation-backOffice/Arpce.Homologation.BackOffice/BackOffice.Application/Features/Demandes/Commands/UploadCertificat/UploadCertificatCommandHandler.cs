using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; 

namespace BackOffice.Application.Features.Demandes.Commands.UploadCertificat;

public class UploadCertificatCommandHandler : IRequestHandler<UploadCertificatCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public UploadCertificatCommandHandler(IApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<bool> Handle(UploadCertificatCommand request, CancellationToken cancellationToken)
    {
        // Vérifie que la demande (équipement) existe
        var demande = await _context.Demandes
            .Include(d => d.Dossier) 
            .FirstOrDefaultAsync(d => d.Id == request.DemandeId, cancellationToken);

        if (demande == null)
        {
            throw new Exception($"Demande (équipement) avec l'ID '{request.DemandeId}' introuvable.");
        }

        var file = request.CertificatFile;

        // Lire le contenu du fichier en mémoire
        byte[] fileData;
        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream, cancellationToken);
            fileData = memoryStream.ToArray();
        }

        // Crée une nouvelle entité Attestation
        var attestation = new Attestation
        {
            Id = Guid.NewGuid(),
            IdDemande = demande.Id,
            DateDelivrance = request.DateDelivrance,
            DateExpiration = request.DateExpiration,
            Donnees = fileData, 
            Extension = Path.GetExtension(file.FileName).TrimStart('.')
        };

        _context.Attestations.Add(attestation);

        // Change le statut du dossier parent à "Attestation signée"
         var statutSigne = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == "DossierSigne", cancellationToken);
         if (statutSigne != null) demande.Dossier.IdStatut = statutSigne.Id;

        await _context.SaveChangesAsync(cancellationToken);

        // 5. Journaliser l'action
        await _auditService.LogAsync(
            page: "Gestion des Attestations",
            libelle: $"Téléversement du certificat pour l'équipement '{demande.Equipement}' (dossier {demande.Dossier.Numero}).",
            eventTypeCode: "UPLOAD",
            dossierId: demande.IdDossier);

        return true;
    }
}