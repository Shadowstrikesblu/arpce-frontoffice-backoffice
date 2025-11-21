using FrontOffice.Application.Features.Authentication;
using MediatR;

public class RegisterClientCommand : IRequest<AuthenticationResult>
{
    public string RaisonSociale { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty; 
    public string Password { get; set; } = string.Empty;
    public string ContactNom { get; set; } = string.Empty;
    public string ContactTelephone { get; set; } = string.Empty;
}