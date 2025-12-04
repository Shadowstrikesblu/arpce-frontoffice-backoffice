using MediatR;

namespace FrontOffice.Application.Features.Authentication.Commands.ResetPassword;

public class ResetPasswordCommand : IRequest<bool>
{
    public string Email { get; set; } 
    public string OtpCode { get; set; } = string.Empty;
    public string NouveauMotDePasse { get; set; } = string.Empty;
}