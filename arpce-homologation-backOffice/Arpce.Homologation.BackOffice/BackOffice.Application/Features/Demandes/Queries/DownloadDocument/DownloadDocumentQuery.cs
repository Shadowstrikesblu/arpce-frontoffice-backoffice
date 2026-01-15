using MediatR;
using System;
namespace BackOffice.Application.Features.Demandes.Queries.DownloadDocument
{
    public class DownloadDocumentQuery : IRequest<DownloadDocumentResult>
    {
        public Guid DocumentId { get; set; }
        public string DocumentType { get; set; } 

        public DownloadDocumentQuery(Guid documentId, string documentType)
        {
            DocumentId = documentId;
            DocumentType = documentType;
        }
    }
}
