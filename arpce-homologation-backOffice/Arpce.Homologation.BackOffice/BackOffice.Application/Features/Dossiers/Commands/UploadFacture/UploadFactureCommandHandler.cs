using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Dossiers.Commands.UploadFacture;

/// <summary>
/// Gère la logique de téléversement de la facture.
/// </summary>
public class UploadFactureCommandHandler : IRequestHandler<UploadFactureCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageProvider _fileStorageProvider;
    private readonly IAuditService _auditService;

    public UploadFactureCommandHandler(
        IApplicationDbContext context,
        IFileStorageProvider fileStorageProvider,
        IAuditService auditService)
    {
        _context = context;
        _fileStorageProvider = fileStorageProvider;
        _auditService = auditService;
    }

    public async Task<bool> Handle(UploadFactureCommand request, CancellationToken cancellationToken)
    {
        // Vérifie que le dossier existe
        var dossier = await _context.Dossiers.FindAsync(new object[] { request.DossierId }, cancellationToken);
        if (dossier == null)
        {
            throw new Exception($"Dossier avec l'ID '{request.DossierId}' introuvable.");
        }

        // Valide le fichier
        var file = request.FactureFile;
        if (file == null || file.Length == 0)
        {
            throw new InvalidOperationException("Le fichier de la facture est manquant.");
        }
        if (Path.GetExtension(file.FileName).ToLowerInvariant() != ".pdf")
        {
            throw new InvalidOperationException("La facture doit être un fichier PDF.");
        }

        // Importe le document via la procédure stockée
        await _fileStorageProvider.ImportDocumentDossierAsync(
            file: file,
            nom: $"Facture_{dossier.Numero}",
            type: 2, 
            dossierId: dossier.Id
        );

        // Mettre à jour le statut du dossier si nécessaire (ex: passer à "En attente de paiement")
            var nouveauStatut = await _context.Statuts.FirstOrDefaultAsync(s => s.Code == "DevisPaiement", cancellationToken);
            if (nouveauStatut != null) dossier.IdStatut = nouveauStatut.Id;
            await _context.SaveChangesAsync(cancellationToken);

        // Journaliser l'action
        await _auditService.LogAsync(
            page: "Gestion des Dossiers",
            libelle: $"Téléversement de la facture pour le dossier '{dossier.Numero}'.",
            eventTypeCode: "UPLOAD",
            dossierId: dossier.Id);

        return true;
    }
}