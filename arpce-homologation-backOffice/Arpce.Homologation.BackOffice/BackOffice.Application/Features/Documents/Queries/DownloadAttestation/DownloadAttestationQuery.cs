using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Documents.Queries.DownloadAttestation;

public class DownloadAttestationQuery : IRequest<FileContentResult>
{
    public Guid AttestationId { get; set; }
}

public class DownloadAttestationQueryHandler : IRequestHandler<DownloadAttestationQuery, FileContentResult>
{
    private readonly IApplicationDbContext _context;

    public DownloadAttestationQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<FileContentResult> Handle(DownloadAttestationQuery request, CancellationToken cancellationToken)
    {
        // On charge l'attestation ET sa demande pour construire le nom du fichier
        var attestation = await _context.Attestations
            .AsNoTracking()
            .Include(a => a.Demande)
            .FirstOrDefaultAsync(a => a.Id == request.AttestationId, cancellationToken);

        if (attestation == null || attestation.Donnees == null || attestation.Donnees.Length == 0)
        {
            throw new Exception("Attestation introuvable ou fichier vide.");
        }

        // Construction d'un nom de fichier logique
        var fileName = $"Certificat_{attestation.Demande.Equipement}_{attestation.NumeroSequentiel}.{attestation.Extension}";

        return new FileContentResult(attestation.Donnees, "application/pdf")
        {
            FileDownloadName = fileName
        };
    }
}

// Classe de retour (simple wrapper)
public class FileContentResult
{
    public byte[] FileContents { get; }
    public string ContentType { get; }
    public string FileDownloadName { get; set; }

    public FileContentResult(byte[] fileContents, string contentType)
    {
        FileContents = fileContents;
        ContentType = contentType;
        FileDownloadName = "document.pdf"; 
    }
}