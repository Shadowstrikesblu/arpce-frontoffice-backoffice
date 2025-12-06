using FrontOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FrontOffice.Application.Features.Documents.Queries.DownloadCertificat;

/// <summary>
/// Gère la logique de récupération du contenu binaire d'une attestation pour le téléchargement.
/// </summary>
public class DownloadCertificatQueryHandler : IRequestHandler<DownloadCertificatQuery, DownloadCertificatResult>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DownloadCertificatQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<DownloadCertificatResult> Handle(DownloadCertificatQuery request, CancellationToken cancellationToken)
    {
        // Vérification de l'utilisateur
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("Utilisateur non authentifié.");
        }

        // Requête pour trouver l'attestation et vérifier les droits d'accès
        // On s'assure que l'attestation appartient à une demande, qui appartient à un dossier, qui appartient au client.
        var attestation = await _context.Attestations
            .AsNoTracking()
            .Include(a => a.Demande) 
            .ThenInclude(dem => dem.Dossier) 
            .FirstOrDefaultAsync(a => a.Id == request.AttestationId && a.Demande.Dossier.IdClient == userId.Value, cancellationToken);

        // Vérification de l'existence
        if (attestation == null || attestation.Donnees == null || attestation.Donnees.Length == 0)
        {
            throw new FileNotFoundException("Le certificat est introuvable ou son contenu est vide.");
        }

        // Construction du nom de fichier
        var fileName = $"Certificat_{attestation.Demande.Equipement}_{attestation.DateDelivrance:yyyy-MM-dd}.{attestation.Extension}";

        // Retourne le résultat
        return new DownloadCertificatResult
        {
            FileContents = attestation.Donnees,
            ContentType = GetMimeType(attestation.Extension),
            FileName = fileName
        };
    }

    /// <summary>
    /// Méthode utilitaire pour déterminer le type MIME.
    /// </summary>
    private string GetMimeType(string extension)
    {
        var ext = extension.TrimStart('.').ToLowerInvariant();
        return ext switch
        {
            "pdf" => "application/pdf",
            _ => "application/octet-stream",
        };
    }
}