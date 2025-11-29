using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities; 
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Admin.Queries.GetAdminUsersList;

/// <summary>
/// Gère la récupération de la liste paginée des utilisateurs administrateurs.
/// </summary>
public class GetAdminUsersListQueryHandler : IRequestHandler<GetAdminUsersListQuery, AdminUserListVm>
{
    private readonly IApplicationDbContext _context;

    public GetAdminUsersListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AdminUserListVm> Handle(GetAdminUsersListQuery request, CancellationToken cancellationToken)
    {
        // Création de la requête de base
        IQueryable<AdminUtilisateur> query = _context.AdminUtilisateurs.AsNoTracking();

        // Filtrage (Recherche)
        if (!string.IsNullOrWhiteSpace(request.Recherche))
        {
            var term = request.Recherche.ToLower();
            query = query.Where(u =>
                u.Compte.ToLower().Contains(term) ||
                u.Nom.ToLower().Contains(term) ||
                (u.Prenoms != null && u.Prenoms.ToLower().Contains(term))
            );
        }

        // Tri
        if (request.Ordre?.ToLower() == "asc")
        {
            query = query.OrderBy(u => u.DateCreation);
        }
        else
        {
            query = query.OrderByDescending(u => u.DateCreation);
        }

        // Pagination et Chargement
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageTaille)
            .Take(request.PageTaille)
            .Include(u => u.UtilisateurType) 
            .ToListAsync(cancellationToken);

        // Mapping
        var userDtos = items.Select(u => new AdminUserListItemDto
        {
            Id = u.Id,
            Compte = u.Compte,
            Nom = u.Nom,
            Prenoms = u.Prenoms,
            ChangementMotPasse = u.ChangementMotPasse,
            Desactive = u.Desactive,
            Remarques = u.Remarques,
            DerniereConnexion = u.DerniereConnexion,
            DateCreation = u.DateCreation,
            TypeUtilisateur = u.UtilisateurType != null ? new AdminUserTypeSimpleDto
            {
                Libelle = u.UtilisateurType.Libelle
            } : null
        }).ToList();

        return new AdminUserListVm
        {
            Page = request.Page,
            PageTaille = request.PageTaille,
            TotalPage = (int)Math.Ceiling(totalCount / (double)request.PageTaille),
            Utilisateurs = userDtos
        };
    }
}