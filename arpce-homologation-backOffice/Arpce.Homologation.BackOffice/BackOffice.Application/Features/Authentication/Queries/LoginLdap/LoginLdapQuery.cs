using MediatR;

namespace BackOffice.Application.Features.Authentication.Queries.LoginLdap;

public class LoginLdapQuery : IRequest<AuthenticationResult>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}