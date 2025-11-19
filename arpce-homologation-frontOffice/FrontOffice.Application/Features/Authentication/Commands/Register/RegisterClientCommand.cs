using MediatR;

namespace FrontOffice.Application.Features.Authentication.Commands.Register;

public class RegisterClientCommand : IRequest<AuthenticationResult>
{
    public string RaisonSociale { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ContactNom { get; set; }
    public string ContactTelephone { get; set; }
}