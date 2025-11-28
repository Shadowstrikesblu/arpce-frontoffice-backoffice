namespace BackOffice.Application.Features.Documents.Queries.GetAttestationsList
{
    public class DossierSimpleDto
    {
        public string Numero { get; set; } = string.Empty;
        public string Libelle { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty; // URL du dossier
    }
}
