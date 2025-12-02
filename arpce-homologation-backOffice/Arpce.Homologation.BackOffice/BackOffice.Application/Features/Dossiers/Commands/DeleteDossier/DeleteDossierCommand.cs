using MediatR;

namespace BackOffice.Application.Features.Dossiers.Commands.DeleteDossier;

public class DeleteDossierCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public DeleteDossierCommand(Guid id) => Id = id;
}