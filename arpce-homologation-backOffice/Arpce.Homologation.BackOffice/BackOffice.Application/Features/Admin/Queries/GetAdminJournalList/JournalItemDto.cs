namespace BackOffice.Application.Features.Admin.Queries.GetAdminJournalList
{
    public class JournalItemDto
    {
        public Guid Id { get; set; }
        public string Application { get; set; } = string.Empty;
        public string AdresseIP { get; set; } = string.Empty;
        public string Utilisateur { get; set; } = string.Empty;
        public DateTime DateEvenement { get; set; }
        public string Page { get; set; } = string.Empty;
        public string? Libelle { get; set; }
        public EvenementTypeDto? EvenementType { get; set; }
    }
}
