using BackOffice.Application.Common.Exceptions;
using BackOffice.Application.Common.Interfaces;
using BackOffice.Application.Common; // Pour l'extension FromUnixTimeMilliseconds
using BackOffice.Application.Features.Admin.Queries.GetRedevablesList;
using BackOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Admin.Queries.GetRedevablesAValider;

/// <summary>
/// Gère la récupération de la liste paginée des redevables en attente de validation.
/// </summary>
public class GetRedevablesAValiderQueryHandler : IRequestHandler<GetRedevablesAValiderQuery, RedevableListVm>
{
    private readonly IApplicationDbContext _context;

    public GetRedevablesAValiderQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RedevableListVm> Handle(GetRedevablesAValiderQuery request, CancellationToken cancellationToken)
    {
        // Requête de base avec le filtre principal : NiveauValidation == 1
        IQueryable<Client> query = _context.Clients
            .AsNoTracking()
            .Where(c => c.NiveauValidation == 1);

        // Filtre de recherche
        if (!string.IsNullOrWhiteSpace(request.Recherche))
        {
            var term = request.Recherche.ToLower();
            query = query.Where(c =>
                c.RaisonSociale.ToLower().Contains(term) ||
                c.Code.ToLower().Contains(term) ||
                (c.Email != null && c.Email.ToLower().Contains(term))
            );
        }

        // Tri
        if (request.Ordre?.ToLower() == "asc")
        {
            query = query.OrderBy(c => c.DateCreation);
        }
        else
        {
            query = query.OrderByDescending(c => c.DateCreation);
        }

        // Comptage
        var totalCount = await query.CountAsync(cancellationToken);

        // Pagination et Chargement des données
        var items = await query
            .Skip((request.Page - 1) * request.PageTaille)
            .Take(request.PageTaille)
            .Include(c => c.Dossiers)
            .ToListAsync(cancellationToken);

        // Mapping vers le DTO
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
            RaisonSociale = c.RaisonSociale,
            DateCreation = c.DateCreation.FromUnixTimeMilliseconds(),
            NbDossier = c.Dossiers.Count,

            // --- AJOUT DEMANDÉ ---
            NiveauValidation = c.NiveauValidation
            // ---------------------
        }).ToList();

        return new RedevableListVm
        {
            Page = request.Page,
            PageTaille = request.PageTaille,
            TotalPage = (int)System.Math.Ceiling(totalCount / (double)request.PageTaille),
            Utilisateur = dtos
        };
    }
}