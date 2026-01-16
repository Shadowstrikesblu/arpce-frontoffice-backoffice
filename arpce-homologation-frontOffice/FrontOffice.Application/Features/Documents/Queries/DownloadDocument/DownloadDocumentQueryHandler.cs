using FrontOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace FrontOffice.Application.Features.Documents.Queries.DownloadDocument;

public class DownloadDocumentQueryHandler : IRequestHandler<DownloadDocumentQuery, DownloadDocumentResult>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DownloadDocumentQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<DownloadDocumentResult> Handle(DownloadDocumentQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue) throw new UnauthorizedAccessException();

        byte[]? fileContents = null;
        string fileName = "file";
        string extension = "bin";

        if (request.DocumentType.Equals("dossier", StringComparison.OrdinalIgnoreCase))
        {
            var document = await _context.DocumentsDossiers
                .AsNoTracking()
                .Include(dd => dd.Dossier)
                .FirstOrDefaultAsync(dd => dd.Id == request.DocumentId && dd.Dossier.IdClient == userId.Value, cancellationToken);

            if (document == null) throw new FileNotFoundException("Document de dossier introuvable.");

            fileContents = document.Donnees;
            fileName = document.Nom ?? "document_dossier";
            extension = document.Extension;
        }
        else if (request.DocumentType.Equals("demande", StringComparison.OrdinalIgnoreCase))
        {
            var document = await _context.DocumentsDemandes
                .AsNoTracking()
                .Include(dd => dd.Demande.Dossier)
                .FirstOrDefaultAsync(dd => dd.Id == request.DocumentId && dd.Demande.Dossier.IdClient == userId.Value, cancellationToken);

            if (document == null) throw new FileNotFoundException("Document de demande introuvable.");

            fileContents = document.Donnees;
            fileName = document.Nom ?? "document_demande";
            extension = document.Extension;
        }

        if (fileContents == null) throw new FileNotFoundException("Contenu du fichier introuvable.");

        return new DownloadDocumentResult
        {
            FileContents = fileContents,
            ContentType = GetMimeType(extension),
            FileName = $"{fileName}.{extension}"
        };
    }

    private string GetMimeType(string extension)
    {
        return extension.ToLowerInvariant() switch
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