using BackOffice.Application.Common.DTOs;
using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Categories.Queries.GetCategoriesList;

public class GetCategoriesListQueryHandler : IRequestHandler<GetCategoriesListQuery, List<CategorieEquipementDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCategoriesListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategorieEquipementDto>> Handle(GetCategoriesListQuery request, CancellationToken cancellationToken)
    {
        var query = _context.CategoriesEquipements.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.TypeEquipement))
        {
            query = query.Where(c => c.TypeEquipement == request.TypeEquipement);
        }

        var categories = await query
            .Select(c => new CategorieEquipementDto
            {
                Id = c.Id,
                Code = c.Code,
                Libelle = c.Libelle,
                TypeEquipement = c.TypeEquipement,
                TypeClient = c.TypeClient,
                FraisEtude = c.FraisEtude,
                FraisHomologation = c.FraisHomologation,
                FraisControle = c.FraisControle,
                FormuleHomologation = c.FormuleHomologation,
                QuantiteReference = c.QuantiteReference,
                Remarques = c.Remarques,
                ModeCalcul = c.ModeCalcul,
                BlockSize = c.BlockSize,
                QtyMin = c.QtyMin,
                QtyMax = c.QtyMax,
                ReferenceLoiFinance = c.ReferenceLoiFinance
            })
            .ToListAsync(cancellationToken);

        return categories;
    }
}