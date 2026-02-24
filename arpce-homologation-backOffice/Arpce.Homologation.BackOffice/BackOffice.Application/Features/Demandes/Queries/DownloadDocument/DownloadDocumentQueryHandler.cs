using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Demandes.Queries.DownloadDocument
{
    /// <summary>
    /// Gère la logique de récupération du contenu binaire d'un document pour le téléchargement.
    /// Version Back Office : L'agent a accès à tous les documents, incluant les devis générés.
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
            string extension = "bin";

            // Détermine dans quelle table chercher le document en fonction du type
            if (request.DocumentType.Equals("dossier", StringComparison.OrdinalIgnoreCase))
            {
                // Recherche dans la table des documents directement liés au dossier
                var document = await _context.DocumentsDossiers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(dd => dd.Id == request.DocumentId, cancellationToken);

                if (document == null)
                {
                    throw new FileNotFoundException($"Document de dossier avec l'ID '{request.DocumentId}' introuvable.");
                }

                fileContents = document.Donnees;
                fileName = document.Nom ?? $"dossier_{document.Id}";
                extension = document.Extension;
            }
            else if (request.DocumentType.Equals("demande", StringComparison.OrdinalIgnoreCase))
            {
                // Recherche dans la table des documents liés à la demande (équipement)
                var document = await _context.DocumentsDemandes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(dd => dd.Id == request.DocumentId, cancellationToken);

                if (document == null)
                {
                    throw new FileNotFoundException($"Document de demande avec l'ID '{request.DocumentId}' introuvable.");
                }

                fileContents = document.Donnees;
                fileName = document.Nom ?? $"demande_{document.Id}";
                extension = document.Extension;
            }
            else if (request.DocumentType.Equals("devis", StringComparison.OrdinalIgnoreCase))
            {
                // --- LOGIQUE POUR LES DEVIS ---
                // 1. On cherche d'abord la ligne dans la table 'devis' pour obtenir l'IdDossier
                var devisRecord = await _context.Devis
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.Id == request.DocumentId, cancellationToken);

                if (devisRecord == null)
                {
                    throw new FileNotFoundException($"Aucun enregistrement de devis trouvé pour l'ID '{request.DocumentId}'.");
                }

                // 2. On cherche le PDF généré dans documentsDossiers (Type 2 = Devis PDF)
                var document = await _context.DocumentsDossiers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(dd => dd.IdDossier == devisRecord.IdDossier && dd.Type == 2, cancellationToken);

                if (document == null)
                {
                    throw new FileNotFoundException($"Le fichier PDF pour le devis {request.DocumentId} n'a pas encore été généré.");
                }

                fileContents = document.Donnees;
                fileName = document.Nom ?? $"devis_{devisRecord.Id}";
                extension = document.Extension;
            }
            else
            {
                // Si le type de document n'est pas reconnu
                throw new ArgumentException("Type de document non valide. Utilisez 'dossier', 'demande' ou 'devis'.");
            }

            // Vérifie si le contenu binaire a bien été trouvé en base
            if (fileContents == null || fileContents.Length == 0)
            {
                throw new FileNotFoundException("Le contenu du fichier est vide ou introuvable dans la base de données.");
            }

            // Construit le résultat final pour le contrôleur
            return new DownloadDocumentResult
            {
                FileContents = fileContents,
                ContentType = GetMimeType(extension),
                FileName = fileName.EndsWith($".{extension}", StringComparison.OrdinalIgnoreCase)
                           ? fileName
                           : $"{fileName}.{extension}"
            };
        }

        /// <summary>
        /// Méthode utilitaire pour déterminer le type MIME à partir de l'extension du fichier.
        /// </summary>
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
}