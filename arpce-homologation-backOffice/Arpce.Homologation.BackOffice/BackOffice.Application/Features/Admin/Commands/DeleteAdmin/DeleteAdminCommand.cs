using MediatR;
using System;

namespace BackOffice.Application.Features.Admin.Commands.DeleteAdmin;

public class DeleteAdminCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public DeleteAdminCommand(Guid id) => Id = id;
}