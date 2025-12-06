namespace FrontOffice.Application.Features.Documents.Queries.DownloadCertificat
{
    /// <summary>
    /// DTO contenant les informations nécessaires pour retourner un fichier au client.
    /// </summary>
    public class DownloadCertificatResult
    {
        public byte[] FileContents { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = "application/octet-stream";
        public string FileName { get; set; } = string.Empty;
    }
}
