using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BackOffice.Application.Common.Exceptions;
using BackOffice.Application.Common;

namespace BackOffice.Application.Features.Admin.Queries.GetAdminJournalList;

public class GetAdminJournalListQueryHandler : IRequestHandler<GetAdminJournalListQuery, JournalListVm>
{
    private readonly IApplicationDbContext _context;

    public GetAdminJournalListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<JournalListVm> Handle(GetAdminJournalListQuery request, CancellationToken cancellationToken)
    {
        IQueryable<AdminJournal> query = _context.AdminJournals.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Recherche))
        {
            var term = request.Recherche.ToLower();
            query = query.Where(j => j.Utilisateur.ToLower().Contains(term) || (j.Libelle != null && j.Libelle.ToLower().Contains(term)));
        }

        if (request.Ordre?.ToLower() == "asc")
        {
            query = query.OrderBy(j => j.DateEvenement);
        }
        else
        {
            query = query.OrderByDescending(j => j.DateEvenement);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageTaille)
            .Take(request.PageTaille)
            .Include(j => j.EvenementType)
            .ToListAsync(cancellationToken);

        var dtos = items.Select(j => new JournalItemDto
        {
            Id = j.Id,
            Application = j.Application,
            AdresseIP = j.AdresseIP,
            Utilisateur = j.Utilisateur,
            DateEvenement = j.DateEvenement.FromUnixTimeMilliseconds(),
            Page = j.Page,
            Libelle = j.Libelle,
            EvenementType = j.EvenementType != null ? new EvenementTypeDto
            {
                Id = j.EvenementType.Id,
                Libelle = j.EvenementType.Libelle
            } : null
        }).ToList();

        return new JournalListVm
        {
            Page = request.Page,
            PageTaille = request.PageTaille,
            TotalPage = (int)Math.Ceiling(totalCount / (double)request.PageTaille),
            Journal = dtos
        };
    }
}