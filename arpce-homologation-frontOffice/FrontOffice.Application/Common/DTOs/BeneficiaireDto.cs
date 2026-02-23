namespace FrontOffice.Application.Common.DTOs;

public class BeneficiaireDto
{
    public string Nom { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telephone { get; set; }
    public string? Type { get; set; } 
    public string? Adresse { get; set; }
}