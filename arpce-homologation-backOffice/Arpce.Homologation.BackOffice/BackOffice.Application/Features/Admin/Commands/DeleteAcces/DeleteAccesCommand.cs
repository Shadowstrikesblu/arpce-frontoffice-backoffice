using MediatR;

namespace BackOffice.Application.Features.Admin.Commands.DeleteAcces;

public class DeleteAccesCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public DeleteAccesCommand(Guid id) => Id = id;
    public string Code { get; set; }
}