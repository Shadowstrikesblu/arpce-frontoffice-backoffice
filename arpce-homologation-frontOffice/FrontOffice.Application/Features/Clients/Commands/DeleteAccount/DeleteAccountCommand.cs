using MediatR;

namespace FrontOffice.Application.Features.Clients.Commands.DeleteAccount;

public class DeleteAccountCommand : IRequest<bool>
{
    public string Password { get; set; } = string.Empty;
}