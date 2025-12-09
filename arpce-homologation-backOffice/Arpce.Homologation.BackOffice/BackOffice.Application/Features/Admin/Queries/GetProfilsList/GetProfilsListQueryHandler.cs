using BackOffice.Application.Common.DTOs;
using BackOffice.Application.Common.Exceptions;
using BackOffice.Application.Common.Interfaces;
using BackOffice.Application.Features.Authentication.Queries.CheckToken;
using BackOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Admin.Queries.GetProfilsList;

public class GetProfilsListQueryHandler : IRequestHandler<GetProfilsListQuery, ProfilsListVm>
{
    private readonly IApplicationDbContext _context;

    public GetProfilsListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProfilsListVm> Handle(GetProfilsListQuery request, CancellationToken cancellationToken)
    {
        IQueryable<AdminProfils> query = _context.AdminProfils.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Recherche))
        {
            var term = request.Recherche.ToLower();
            query = query.Where(p => p.Code.ToLower().Contains(term) || p.Libelle.ToLower().Contains(term));
        }

        query = request.Ordre?.ToLower() == "asc" ? query.OrderBy(p => p.Libelle) : query.OrderByDescending(p => p.Libelle);

        var total = await query.CountAsync(cancellationToken);

        var profils = await query
            .Skip((request.Page - 1) * request.PageTaille)
            .Take(request.PageTaille)
            .Include(p => p.Utilisateurs)
            .Include(p => p.AdminProfilsAcces).ThenInclude(apa => apa.Access)
            .Include(p => p.AdminProfilsUtilisateursLDAP)
            .ToListAsync(cancellationToken);

        var dtos = profils.Select(p => new ProfilFullDto
        {
            Id = p.Id,
            Code = p.Code,
            Libelle = p.Libelle,
            Remarques = p.Remarques,
            UtilisateurCreation = p.UtilisateurCreation,
            DateCreation = p.DateCreation.FromUnixTimeMilliseconds(),
            Utilisateurs = p.Utilisateurs.Select(u => new AdminUserSimpleDto { Id = u.Id, Compte = u.Compte, Nom = u.Nom, Prenoms = u.Prenoms }).ToList(),
            UtilisateursLDAP = p.AdminProfilsUtilisateursLDAP.Select(l => new AdminProfilsUtilisateursLDAPDto { Utilisateur = l.Utilisateur, IdProfil = l.IdProfil }).ToList(),
            Acces = p.AdminProfilsAcces.Select(a => new AdminProfilAccesDto
            {
                IdProfil = a.IdProfil,
                IdAccess = a.IdAccess,
                Ajouter = a.Ajouter == 1,
                Valider = a.Valider == 1,
                Supprimer = a.Supprimer == 1,
                Imprimer = a.Imprimer == 1,
                Access = a.Access != null ? new AdminAccessDto { Id = a.Access.Id, Application = a.Access.Application, Groupe = a.Access.Groupe, Libelle = a.Access.Libelle } : null
            }).ToList()
        }).ToList();

        return new ProfilsListVm
        {
            Page = request.Page,
            PageTaille = request.PageTaille,
            TotalPage = (int)System.Math.Ceiling(total / (double)request.PageTaille),
            Profils = dtos
        };
    }
}