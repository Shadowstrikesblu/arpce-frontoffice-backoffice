using FrontOffice.Application.Features.Documents.Queries.DownloadDocument;
using MediatR;

public class DownloadDocumentQuery : IRequest<DownloadDocumentResult>
{
    public Guid DocumentId { get; set; }
    public string DocumentType { get; set; } // "dossier" ou "demande"

    public DownloadDocumentQuery(Guid documentId, string documentType)
    {
        DocumentId = documentId;
        DocumentType = documentType;
    }
}