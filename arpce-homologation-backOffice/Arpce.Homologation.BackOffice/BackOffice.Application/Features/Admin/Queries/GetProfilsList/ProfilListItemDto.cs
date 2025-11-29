namespace BackOffice.Application.Features.Admin.Queries.GetProfilsList
{
    public class ProfilListItemDto
    {
        public Guid Id { get; set; }
        public string? Code { get; set; }
        public string Libelle { get; set; } = string.Empty;
        public string? Remarques { get; set; }
        public string? UtilisateurCreation { get; set; }
        public DateTime? DateCreation { get; set; }
        public string? UtilisateurModification { get; set; }
        public DateTime? DateModification { get; set; }

        // Compteurs
        public int NbAcces { get; set; }
        public int UtilisateursLDAP { get; set; }
        public int Utilisateurs { get; set; }
    }
}
