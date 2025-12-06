using BackOffice.Application.Common.DTOs;
using BackOffice.Application.Common.Interfaces;
using BackOffice.Application.Features.Authentication.Queries.CheckToken;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Admin.Queries.GetAccessList;

public class GetAccessListQueryHandler : IRequestHandler<GetAccessListQuery, List<AdminAccessDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAccessListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AdminAccessDto>> Handle(GetAccessListQuery request, CancellationToken cancellationToken)
    {
        // Récupère tous les accès, triés par Application puis Groupe
        return await _context.AdminAccesses
            .AsNoTracking()
            .OrderBy(a => a.Application)
            .ThenBy(a => a.Groupe)
            .Select(a => new AdminAccessDto
            {
                Id = a.Id,
                Application = a.Application,
                Groupe = a.Groupe,
                Libelle = a.Libelle,
                Page = a.Page,
                Type = a.Type,
                Inactif = a.Inactif == 1,
                Ajouter = a.Ajouter == 1,
                Valider = a.Valider == 1,
                Supprimer = a.Supprimer == 1,
                Imprimer = a.Imprimer == 1
            })
            .ToListAsync(cancellationToken);
    }
}