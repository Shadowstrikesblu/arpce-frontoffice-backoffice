using MediatR;

namespace BackOffice.Application.Features.Admin.Commands.CreateProfil;

public class CreateProfilCommand : IRequest<bool>
{
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public string? Remarques { get; set; }
    public List<ProfilAccessDto> Access { get; set; } = new List<ProfilAccessDto>();
}