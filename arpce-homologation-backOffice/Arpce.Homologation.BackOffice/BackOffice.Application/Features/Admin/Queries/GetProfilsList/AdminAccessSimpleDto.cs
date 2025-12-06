
namespace BackOffice.Application.Features.Admin.Queries.GetProfilsList
{
    public class AdminAccessSimpleDto
    {
        public Guid Id { get; set; }
        public string Application { get; set; } = string.Empty;
        public string Groupe { get; set; } = string.Empty;
        public string Libelle { get; set; } = string.Empty;
    }
}
