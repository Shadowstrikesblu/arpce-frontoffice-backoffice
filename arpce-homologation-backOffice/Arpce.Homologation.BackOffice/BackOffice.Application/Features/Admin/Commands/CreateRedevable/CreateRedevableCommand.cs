using MediatR;

namespace BackOffice.Application.Features.Admin.Commands.CreateRedevable;

public class CreateRedevableCommand : IRequest<bool>
{
    public string Code { get; set; } = string.Empty;
    public string RaisonSociale { get; set; } = string.Empty;
    public string? RegistreCommerce { get; set; }
    public string? MotPasse { get; set; }
    public bool? Desactive { get; set; }
    public string? ContactNom { get; set; }
    public string? ContactTelephone { get; set; }
    public string? ContactFonction { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Adresse { get; set; }
    public string? Bp { get; set; }
    public string? Ville { get; set; }
    public string? Pays { get; set; }
    public string? Remarques { get; set; }
}