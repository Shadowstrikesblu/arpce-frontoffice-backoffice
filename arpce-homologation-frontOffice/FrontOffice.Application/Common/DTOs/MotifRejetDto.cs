namespace FrontOffice.Application.Common.DTOs;
public class MotifRejetDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public string? Remarques { get; set; }
}