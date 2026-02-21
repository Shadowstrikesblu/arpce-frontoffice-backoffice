using FrontOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FrontOffice.Application.Features.Demandes.Commands.UploadDossierFiles;

public class UploadDossierFilesCommandHandler : IRequestHandler<UploadDossierFilesCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageProvider _fileStorage;
    private readonly ILogger<UploadDossierFilesCommandHandler> _logger;

    public UploadDossierFilesCommandHandler(
        IApplicationDbContext context,
        IFileStorageProvider fileStorage,
        ILogger<UploadDossierFilesCommandHandler> logger)
    {
        _context = context;
        _fileStorage = fileStorage;
        _logger = logger;
    }

    public async Task<bool> Handle(UploadDossierFilesCommand request, CancellationToken cancellationToken)
    {
        // Récupération du dossier avec son unique demande (échantillon 1:1)
        var dossier = await _context.Dossiers
            .Include(d => d.Demande)
            .FirstOrDefaultAsync(d => d.Id == request.DossierId, cancellationToken);

        if (dossier == null || dossier.Demande == null)
        {
            throw new Exception("Dossier ou équipement introuvable.");
        }

        // 2. Upload de la Lettre de Demande (Liée au dossier - Type 0)
        if (request.LettreDemande != null)
        {
            await _fileStorage.ImportDocumentDossierAsync(
                request.LettreDemande,
                $"Lettre_Demande_{dossier.Numero}",
                0,
                dossier.Id);
        }

        // Upload de la Fiche Technique (Liée à la demande/équipement)
        if (request.FicheTechnique != null)
        {
            await _fileStorage.ImportDocumentDemandeAsync(
                request.FicheTechnique,
                $"FicheTechnique_{dossier.Demande.Equipement}",
                dossier.Demande.Id);
        }

        // Upload des documents supplémentaires (Annexes multiples)
        if (request.DocumentsSupplementaires != null && request.DocumentsSupplementaires.Any())
        {
            foreach (var file in request.DocumentsSupplementaires)
            {
                await _fileStorage.ImportDocumentDemandeAsync(
                    file,
                    file.FileName,
                    dossier.Demande.Id);
            }
        }

        // Sauvegarde finale
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Fichiers enregistrés avec succès pour le dossier {Numero}", dossier.Numero);

        return true;
    }
}