namespace BackOffice.Application.Features.Admin.Queries.GetRedevableDetail
{
    public class DossierRedevableDto
    {
        public Guid Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public string Libelle { get; set; } = string.Empty;
        public DateTime DateOuverture { get; set; }
        public string StatutLibelle { get; set; } = string.Empty;
        public int NombreEquipements { get; set; }
    }
}
