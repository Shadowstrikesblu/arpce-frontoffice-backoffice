using MediatR;

namespace FrontOffice.Application.Features.Authentication.Queries.Login;

public class LoginClientQuery : IRequest<AuthenticationResult>
{
    public string Email { get; set; }
    public string Password { get; set; }
}