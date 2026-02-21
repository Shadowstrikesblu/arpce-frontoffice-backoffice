using BackOffice.Application.Common.DTOs;
using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Signataires.Queries.GetSignatairesList;

public class GetSignatairesListQuery : IRequest<SignatairesListVm>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetSignatairesListQueryHandler : IRequestHandler<GetSignatairesListQuery, SignatairesListVm>
{
    private readonly IApplicationDbContext _context;

    public GetSignatairesListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SignatairesListVm> Handle(GetSignatairesListQuery request, CancellationToken cancellationToken)
    {
        var queryable = _context.Signataires
            .Include(s => s.AdminUtilisateur) // Jointure obligatoire
            .AsNoTracking()
            .OrderBy(s => s.AdminUtilisateur.Nom);

        var totalCount = await queryable.CountAsync(cancellationToken);

        var items = await queryable
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new SignataireDto
            {
                Id = s.Id,
                Nom = s.AdminUtilisateur.Nom,      
                Prenoms = s.AdminUtilisateur.Prenoms,
                Fonction = s.AdminUtilisateur.Fonction, 
                SignatureImageUrl = s.SignatureImagePath,
                IsActive = s.IsActive
            })
            .ToListAsync(cancellationToken);

        return new SignatairesListVm
        {
            Items = items,
            PageNumber = request.PageNumber,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };
    }
}