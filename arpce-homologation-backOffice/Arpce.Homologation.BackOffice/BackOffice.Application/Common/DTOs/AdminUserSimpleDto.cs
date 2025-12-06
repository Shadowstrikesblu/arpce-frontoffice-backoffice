namespace BackOffice.Application.Common.DTOs
{
    public class AdminUserSimpleDto
    {
        public Guid Id { get; set; }
        public string Compte { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public string? Prenoms { get; set; }
    }
}
