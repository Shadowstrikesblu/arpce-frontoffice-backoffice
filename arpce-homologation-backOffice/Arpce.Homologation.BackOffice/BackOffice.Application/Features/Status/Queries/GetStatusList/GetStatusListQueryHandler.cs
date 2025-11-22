using BackOffice.Application.Common.DTOs;
using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Status.Queries.GetStatusList;

/// <summary>
/// Gère la logique de la requête pour récupérer la liste des statuts.
/// </summary>
public class GetStatusListQueryHandler : IRequestHandler<GetStatusListQuery, List<StatutDto>>
{
    private readonly IApplicationDbContext _context;

    public GetStatusListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Exécute la requête et retourne la liste des statuts.
    /// </summary>
    public async Task<List<StatutDto>> Handle(GetStatusListQuery request, CancellationToken cancellationToken)
    {
        // Utilisation de AsNoTracking() pour optimiser la performance en lecture seule.
        var statusList = await _context.Statuts
            .AsNoTracking()
            .OrderBy(s => s.Libelle) 
            .Select(s => new StatutDto
            {
                Id = s.Id,
                Code = s.Code,
                Libelle = s.Libelle
            })
            .ToListAsync(cancellationToken);

        return statusList;
    }
}