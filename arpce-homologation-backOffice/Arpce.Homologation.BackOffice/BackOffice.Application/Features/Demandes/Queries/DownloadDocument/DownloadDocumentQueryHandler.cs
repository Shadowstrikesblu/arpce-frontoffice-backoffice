using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Demandes.Queries.DownloadDocument
{
    /// <summary>
    /// Gère la logique de récupération du contenu binaire d'un document pour le téléchargement.
    /// Version Back Office : L'agent a accès à tous les documents.
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
                // Recherche dans la table des documents liés au dossier
                var document = await _context.DocumentsDossiers
                    .AsNoTracking() // Optimisation
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
            else
            {
                // Si le type de document n'est ni "dossier" ni "demande"
                throw new ArgumentException("Type de document non valide. Utilisez 'dossier' ou 'demande'.");
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
                FileName = $"{fileName}.{extension}"
            };
        }

        /// <summary>
        /// Méthode utilitaire pour déterminer le type MIME à partir de l'extension du fichier.
        /// </summary>
        private string GetMimeType(string extension)
        {
            // On enlève le point s'il est présent pour la comparaison
            var ext = extension.TrimStart('.').ToLowerInvariant();

            return ext switch
            {
                "pdf" => "application/pdf",
                "jpg" or "jpeg" => "image/jpeg",
                "png" => "image/png",
                "doc" => "application/msword",
                "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                // Type par défaut pour tous les autres cas (force le téléchargement)
                _ => "application/octet-stream",
            };
        }
    }
}
