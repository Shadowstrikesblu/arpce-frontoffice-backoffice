namespace FrontOffice.Application.Common.DTOs;
public class PropositionDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
}