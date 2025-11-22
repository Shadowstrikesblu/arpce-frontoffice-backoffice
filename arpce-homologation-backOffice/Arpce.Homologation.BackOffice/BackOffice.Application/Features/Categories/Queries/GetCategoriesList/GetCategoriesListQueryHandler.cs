using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Categories.Queries.GetCategoriesList;

/// <summary>
/// Gère la logique de la requête pour récupérer la liste des catégories d'équipement.
/// </summary>
public class GetCategoriesListQueryHandler : IRequestHandler<GetCategoriesListQuery, List<CategorieEquipementDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCategoriesListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Exécute la requête de récupération des catégories.
    /// </summary>
    public async Task<List<CategorieEquipementDto>> Handle(GetCategoriesListQuery request, CancellationToken cancellationToken)
    {
        // On commence avec une requête IQueryable de base.
        var query = _context.CategoriesEquipements.AsNoTracking();

        // Si un type d'équipement est fourni en paramètre, on applique le filtre.
        if (!string.IsNullOrWhiteSpace(request.TypeEquipement))
        {
            query = query.Where(c => c.TypeEquipement == request.TypeEquipement);
        }

        // On exécute la requête et on mappe les résultats vers le DTO.
        var categories = await query
            .Select(c => new CategorieEquipementDto
            {
                Id = c.Id,
                Code = c.Code,
                Libelle = c.Libelle,
                TypeEquipement = c.TypeEquipement,
                TypeClient = c.TypeClient,
                FraisEtude = c.TarifEtude,
                FraisHomologation = c.TarifHomologation,
                FraisControle = c.TarifControle,
                FormuleHomologation = c.FormuleHomologation,
                QuantiteReference = c.QuantiteReference,
                Remarques = c.Remarques
            })
            .ToListAsync(cancellationToken);

        return categories;
    }
}