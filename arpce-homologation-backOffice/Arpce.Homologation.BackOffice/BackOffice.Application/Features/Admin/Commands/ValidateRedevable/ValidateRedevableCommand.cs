using MediatR;

namespace BackOffice.Application.Features.Admin.Commands.ValidateRedevable;

public class ValidateRedevableCommand : IRequest<bool>
{
    public Guid RedevableId { get; set; }
}