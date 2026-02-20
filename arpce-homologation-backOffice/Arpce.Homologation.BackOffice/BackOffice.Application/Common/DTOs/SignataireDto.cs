namespace BackOffice.Application.Common.DTOs;

public class SignataireDto
{
    public Guid Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenoms { get; set; } = string.Empty;
    public string Fonction { get; set; } = string.Empty;
    public string? SignatureImageUrl { get; set; } 
    public bool IsActive { get; set; }
    public Guid AdminId { get; set; }
}