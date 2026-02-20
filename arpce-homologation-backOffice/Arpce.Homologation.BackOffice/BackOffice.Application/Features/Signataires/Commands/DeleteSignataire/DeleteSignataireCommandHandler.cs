using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Signataires.Commands.DeleteSignataire
{
    public class DeleteSignataireCommandHandler : IRequestHandler<DeleteSignataireCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public DeleteSignataireCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<bool> Handle(DeleteSignataireCommand request, CancellationToken cancellationToken)
        {
            var signataire = await _context.Signataires.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (signataire == null) return false;

            _context.Signataires.Remove(signataire);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
