using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Demandes.Queries.DownloadDocument;

/// <summary>
/// Gère la logique de récupération du contenu binaire d'un document pour le téléchargement.
/// Supporte désormais les types : dossier, demande, devis et recu.
/// </summary>
public class DownloadDocumentQueryHandler : IRequestHandler<DownloadDocumentQuery, DownloadDocumentResult>
{
    private readonly IApplicationDbContext _context;

    public DownloadDocumentQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DownloadDocumentResult> Handle(DownloadDocumentQuery request, CancellationToken cancellationToken)
    {
        byte[]? fileContents = null;
        string fileName = "document";
        string extension = "pdf"; 

        var type = request.DocumentType.ToLower();

        switch (type)
        {
            case "dossier":
                var documentDossier = await _context.DocumentsDossiers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(dd => dd.Id == request.DocumentId, cancellationToken);

                if (documentDossier != null)
                {
                    fileContents = documentDossier.Donnees;
                    fileName = documentDossier.Nom ?? $"dossier_{documentDossier.Id}";
                    extension = documentDossier.Extension;
                }
                break;

            case "demande":
                var documentDemande = await _context.DocumentsDemandes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(dd => dd.Id == request.DocumentId, cancellationToken);

                if (documentDemande != null)
                {
                    fileContents = documentDemande.Donnees;
                    fileName = documentDemande.Nom ?? $"demande_{documentDemande.Id}";
                    extension = documentDemande.Extension;
                }
                break;

            case "devis":
                var devisRecord = await _context.Devis
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.Id == request.DocumentId, cancellationToken);

                if (devisRecord != null)
                {
                    var docDevis = await _context.DocumentsDossiers
                        .AsNoTracking()
                        .FirstOrDefaultAsync(dd => dd.IdDossier == devisRecord.IdDossier && dd.Type == 2, cancellationToken);

                    if (docDevis != null)
                    {
                        fileContents = docDevis.Donnees;
                        fileName = docDevis.Nom ?? $"Devis_{devisRecord.Id}";
                        extension = docDevis.Extension;
                    }
                }
                break;

            case "recu":
                var docRecu = await _context.DocumentsDossiers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(dd => dd.Id == request.DocumentId, cancellationToken);

                if (docRecu != null)
                {
                    fileContents = docRecu.Donnees;
                    fileName = docRecu.Nom ?? $"Recu_{docRecu.Id}";
                    extension = docRecu.Extension;
                }
                break;

            default:
                throw new ArgumentException("Type de document non valide. Utilisez 'dossier', 'demande', 'devis' ou 'recu'.");
        }

        // Vérification finale
        if (fileContents == null || fileContents.Length == 0)
        {
            throw new FileNotFoundException($"Le document de type '{type}' avec l'ID {request.DocumentId} est introuvable ou vide.");
        }

        return new DownloadDocumentResult
        {
            FileContents = fileContents,
            ContentType = GetMimeType(extension),
            FileName = fileName.EndsWith($".{extension}", StringComparison.OrdinalIgnoreCase)
                       ? fileName
                       : $"{fileName}.{extension}"
        };
    }

    private string GetMimeType(string extension)
    {
        var ext = extension.TrimStart('.').ToLowerInvariant();
        return ext switch
        {
            "pdf" => "application/pdf",
            "jpg" or "jpeg" => "image/jpeg",
            "png" => "image/png",
            "doc" => "application/msword",
            "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            _ => "application/octet-stream",
        };
    }
}