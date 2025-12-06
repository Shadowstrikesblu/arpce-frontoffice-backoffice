namespace BackOffice.Application.Features.Admin.Queries.GetProfilsList
{
    public class AdminProfilAccesDto
    {
        public Guid IdProfil { get; set; }
        public Guid IdAccess { get; set; }
        public bool Ajouter { get; set; }
        public bool Valider { get; set; }
        public bool Supprimer { get; set; }
        public bool Imprimer { get; set; }
        public AdminAccessSimpleDto? Access { get; set; }
    }
}
