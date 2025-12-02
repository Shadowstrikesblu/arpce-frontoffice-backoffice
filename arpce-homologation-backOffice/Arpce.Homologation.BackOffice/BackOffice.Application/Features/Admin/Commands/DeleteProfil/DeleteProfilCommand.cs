using MediatR;

namespace BackOffice.Application.Features.Admin.Commands.DeleteProfil;

public class DeleteProfilCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public DeleteProfilCommand(Guid id) => Id = id;
}