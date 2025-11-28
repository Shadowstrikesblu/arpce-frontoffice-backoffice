namespace BackOffice.Application.Features.Admin.Queries.GetUserTypes
{
    public class AdminUserTypeDto
    {
        public Guid Id { get; set; }
        public string Libelle { get; set; } = string.Empty;
    }
}
