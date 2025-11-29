namespace BackOffice.Application.Features.Admin.Queries.GetRedevablesList
{
    public class RedevableListItemDto
    {
        public Guid Id { get; set; } 
        public string Code { get; set; } = string.Empty;
        public bool Desactive { get; set; }
        public string? ContactNom { get; set; }
        public string? ContactTelephone { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Ville { get; set; }
        public string? Pays { get; set; }
        public DateTime? DateCreation { get; set; }
        public int NbDossier { get; set; }
    }
}
