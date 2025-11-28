using MediatR;
namespace BackOffice.Application.Features.Admin.Commands.AssignProfilToLdapUser;

public class AssignProfilToLdapUserCommand : IRequest<bool>
{
    public string Utilisateur { get; set; } = string.Empty; 
    public Guid IdProfil { get; set; }
}