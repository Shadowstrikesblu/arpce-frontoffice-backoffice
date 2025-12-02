using MediatR;
using System;

namespace BackOffice.Application.Features.Admin.Commands.DeleteRedevable;

public class DeleteRedevableCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public DeleteRedevableCommand(Guid id) => Id = id;
}