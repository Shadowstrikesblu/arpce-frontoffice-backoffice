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
        // Récupération du dossier avec son unique demande (1:1)
        var dossier = await _context.Dossiers
            .Include(d => d.Demande)
            .FirstOrDefaultAsync(d => d.Id == request.DossierId, cancellationToken);

        if (dossier == null || dossier.Demande == null)
        {
            throw new Exception("Dossier ou équipement introuvable.");
        }

        // 2. Upload de la Lettre de Demande (Libellé fixe par défaut)
        if (request.LettreDemande != null)
        {
            await _fileStorage.ImportDocumentDossierAsync(
                request.LettreDemande,
                $"Lettre_Demande_{dossier.Numero}",
                0,
                dossier.Id,
                "Lettre de demande d'homologation");
        }

        // Upload de la Fiche Technique (Libellé fixe par défaut)
        if (request.FicheTechnique != null)
        {
            await _fileStorage.ImportDocumentDemandeAsync(
                request.FicheTechnique,
                $"FicheTechnique_{dossier.Demande.Equipement}",
                dossier.Demande.Id,
                "Fiche technique de l'équipement");
        }

        // Upload des documents supplémentaires avec Libellés dynamiques
        if (request.DocumentsSupplementaires != null && request.DocumentsSupplementaires.Any())
        {
            for (int i = 0; i < request.DocumentsSupplementaires.Count; i++)
            {
                var file = request.DocumentsSupplementaires[i];

                string? libelleText = (request.LibellesDocumentsSupplementaires != null && request.LibellesDocumentsSupplementaires.Count > i)
                    ? request.LibellesDocumentsSupplementaires[i]
                    : file.FileName;

                await _fileStorage.ImportDocumentDemandeAsync(
                    file,
                    file.FileName,
                    dossier.Demande.Id,
                    libelleText);
            }
        }

        // Sauvegarde finale en base
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Tous les documents avec libellés ont été enregistrés pour le dossier {Numero}", dossier.Numero);

        return true;
    }
}