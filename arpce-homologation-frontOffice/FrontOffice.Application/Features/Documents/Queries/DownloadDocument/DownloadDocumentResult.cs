namespace FrontOffice.Application.Features.Documents.Queries.DownloadDocument
{
    public class DownloadDocumentResult
    {
        public byte[] FileContents { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = "application/octet-stream";
        public string FileName { get; set; } = string.Empty;
    }
}
