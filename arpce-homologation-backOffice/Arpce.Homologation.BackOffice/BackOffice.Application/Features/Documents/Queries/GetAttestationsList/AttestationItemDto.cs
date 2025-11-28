namespace BackOffice.Application.Features.Documents.Queries.GetAttestationsList
{
    public class AttestationItemDto
    {
        public DateTime DateDelivrance { get; set; }
        public DateTime DateExpiration { get; set; }
        public string Extension { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public DossierSimpleDto Dossier { get; set; } = new();
    }
}
