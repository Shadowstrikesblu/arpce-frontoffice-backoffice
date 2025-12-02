using BackOffice.Application.Common.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Admin.Commands.DeleteRedevable;

public class DeleteRedevableCommandHandler : IRequestHandler<DeleteRedevableCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteRedevableCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteRedevableCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.FindAsync(new object[] { request.Id }, cancellationToken);
        if (client == null) throw new Exception("Redevable introuvable.");

        // Option A : Suppression physique 
        // _context.Clients.Remove(client); 

        // Option B : Suppression logique (Désactivation) - Souvent préférable
        client.Desactive = 1; // 1 = true

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}