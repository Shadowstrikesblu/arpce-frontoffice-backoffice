namespace BackOffice.Application.Features.Admin.Queries.GetAdminUsersList
{
    /// <summary>
    /// DTO représentant un utilisateur admin dans une liste simplifiée.
    /// </summary>
    public class AdminUserListItemDto
    {
        public Guid Id { get; set; }
        public string Compte { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public string? Prenoms { get; set; }
        public bool ChangementMotPasse { get; set; }
        public bool Desactive { get; set; }
        public string? Remarques { get; set; }
        public DateTime? DerniereConnexion { get; set; }
        public DateTime? DateCreation { get; set; }

        // TypeUtilisateur est un objet imbriqué simple
        public AdminUserTypeSimpleDto? TypeUtilisateur { get; set; }
    }
}
