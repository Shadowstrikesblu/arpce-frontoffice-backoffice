namespace BackOffice.Application.Features.Admin.Queries.GetProfilsList
{
    public class AdminUserSimpleDto
    {
        public Guid Id { get; set; }
        public string Compte { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public string? Prenoms { get; set; }
    }
}
