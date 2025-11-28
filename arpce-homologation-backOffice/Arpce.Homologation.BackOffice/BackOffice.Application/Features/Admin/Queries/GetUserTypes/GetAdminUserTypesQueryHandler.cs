using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Admin.Queries.GetUserTypes;

public class GetAdminUserTypesQueryHandler : IRequestHandler<GetAdminUserTypesQuery, List<AdminUserTypeDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAdminUserTypesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AdminUserTypeDto>> Handle(GetAdminUserTypesQuery request, CancellationToken cancellationToken)
    {
        return await _context.AdminUtilisateurTypes
            .AsNoTracking()
            .Select(t => new AdminUserTypeDto { Id = t.Id, Libelle = t.Libelle })
            .ToListAsync(cancellationToken);
    }
}