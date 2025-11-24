using MediatR;

namespace FrontOffice.Application.Features.Authentication.Commands.ChangePassword;

public class ChangePasswordCommand : IRequest<bool>
{
    public string AncienMotDePasse { get; set; } = string.Empty;
    public string NouveauMotDePasse { get; set; } = string.Empty;
}