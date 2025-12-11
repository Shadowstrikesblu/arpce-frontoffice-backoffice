using BackOffice.Application.Common.Exceptions;
using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities; 
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Admin.Queries.GetRedevablesList;

public class GetRedevablesListQueryHandler : IRequestHandler<GetRedevablesListQuery, RedevableListVm>
{
    private readonly IApplicationDbContext _context;

    public GetRedevablesListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RedevableListVm> Handle(GetRedevablesListQuery request, CancellationToken cancellationToken)
    {
        // Défini la requête de base comme IQueryable<Client> (sans Include pour l'instant)
        IQueryable<Client> query = _context.Clients.AsNoTracking();

        // Applique les filtres de recherche
        if (!string.IsNullOrWhiteSpace(request.Recherche))
        {
            var term = request.Recherche.ToLower();
            query = query.Where(c =>
                c.RaisonSociale.ToLower().Contains(term) ||
                c.Code.ToLower().Contains(term) ||
                (c.Email != null && c.Email.ToLower().Contains(term)) ||
                (c.ContactNom != null && c.ContactNom.ToLower().Contains(term))
            );
        }

        // Applique le tri (toujours sur IQueryable<Client>)
        if (request.Ordre?.ToLower() == "asc")
        {
            query = query.OrderBy(c => c.DateCreation);
        }
        else
        {
            query = query.OrderByDescending(c => c.DateCreation);
        }

        // Compte le total (avant pagination)
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageTaille)
            .Take(request.PageTaille)
            .Include(c => c.Dossiers) 
            .ToListAsync(cancellationToken);

        // Mappe vers le DTO
        var dtos = items.Select(c => new RedevableListItemDto
        {
            Id = c.Id,
            Code = c.Code,
            Desactive = c.Desactive == 1,
            ContactNom = c.ContactNom,
            ContactTelephone = c.ContactTelephone,
            Email = c.Email ?? "",
            Ville = c.Ville,
            Pays = c.Pays,
            DateCreation = c.DateCreation.FromUnixTimeMilliseconds(),
            NbDossier = c.Dossiers.Count,
            NiveauValidation = c.NiveauValidation
        }).ToList();

        return new RedevableListVm
        {
            Page = request.Page,
            PageTaille = request.PageTaille,
            TotalPage = (int)Math.Ceiling(totalCount / (double)request.PageTaille),
            Utilisateur = dtos
        };
    }
}