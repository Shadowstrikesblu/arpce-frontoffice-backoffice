using MediatR;
namespace BackOffice.Application.Features.Admin.Commands.CreateAdmin;

public class CreateAdminCommand : IRequest<bool>
{
    public string Compte { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string? Prenoms { get; set; }
    public string? MotPasse { get; set; }
    public bool Desactive { get; set; }
    public string? Remarques { get; set; }
    public Guid IdUtilisateurType { get; set; }
}