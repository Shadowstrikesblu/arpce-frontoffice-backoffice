using MediatR;

namespace FrontOffice.Application.Features.Authentication.Commands.ForgotPassword
{
    public class ForgotPasswordCommand : IRequest<AuthenticationResult> 
    {
        public string Email { get; set; } = string.Empty;
    }
}
