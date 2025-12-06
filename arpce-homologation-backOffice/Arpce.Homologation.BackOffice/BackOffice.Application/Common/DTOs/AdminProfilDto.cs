namespace BackOffice.Application.Common.DTOs;

public class AdminProfilDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public string? Remarques { get; set; }
    public string? UtilisateurCreation { get; set; }
    public DateTime? DateCreation { get; set; }

    public List<AdminProfilAccesDto> Acces { get; set; } = new List<AdminProfilAccesDto>();
}