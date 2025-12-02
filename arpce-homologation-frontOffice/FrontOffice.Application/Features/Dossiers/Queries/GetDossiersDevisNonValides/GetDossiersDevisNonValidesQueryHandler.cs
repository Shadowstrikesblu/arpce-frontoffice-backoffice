using FrontOffice.Application.Common.DTOs; 
using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Application.Features.Dossiers.Queries.GetDossiersList; 
using FrontOffice.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FrontOffice.Application.Features.Dossiers.Queries.GetDossiersDevisNonValides;

/// <summary>
/// Handler pour récupérer la liste des dossiers dont les devis ont été émis mais ne sont pas encore validés par le client.
/// </summary>
public class GetDossiersDevisNonValidesQueryHandler : IRequestHandler<GetDossiersDevisNonValidesQuery, DossiersListVm>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetDossiersDevisNonValidesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<DossiersListVm> Handle(GetDossiersDevisNonValidesQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("Utilisateur non authentifié.");
        }

        // Statut cible : "Devis émis" (en attente de validation client)
        var statutCible = StatutDossierEnum.DevisEmit.ToString();

        // Construction de la requête de base
        var query = _context.Dossiers.AsNoTracking()
            .Where(d => d.IdClient == userId.Value && d.Statut.Code == statutCible);

        // Application du filtre de recherche (Recherche)
        if (!string.IsNullOrWhiteSpace(request.Parameters.Recherche))
        {
            var term = request.Parameters.Recherche.Trim().ToLower();
            query = query.Where(d =>
                d.Numero.ToLower().Contains(term) ||
                d.Libelle.ToLower().Contains(term)
            );
        }

        // Application du tri (TrierPar, Ordre)
        if (!string.IsNullOrWhiteSpace(request.Parameters.TrierPar))
        {
            bool isDescending = request.Parameters.Ordre?.ToLower() == "desc";

            query = request.Parameters.TrierPar.ToLower() switch
            {
                "date_creation" or "dateouverture" => isDescending
                    ? query.OrderByDescending(d => d.DateOuverture)
                    : query.OrderBy(d => d.DateOuverture),

                "libelle" => isDescending
                    ? query.OrderByDescending(d => d.Libelle)
                    : query.OrderBy(d => d.Libelle),

                "date-update" => isDescending
                    ? query.OrderByDescending(d => d.DateModification)
                    : query.OrderBy(d => d.DateModification),

                _ => query.OrderByDescending(d => d.DateOuverture) 
            };
        }
        else
        {
            // Tri par défaut si non spécifié
            query = query.OrderByDescending(d => d.DateOuverture);
        }

        // Calcul du nombre total
        var totalCount = await query.CountAsync(cancellationToken);

        // Pagination et Chargement des données liées
        var dossiers = await query
            .Include(d => d.Statut)
            .Include(d => d.Devis)
            .Skip((request.Parameters.Page - 1) * request.Parameters.TaillePage)
            .Take(request.Parameters.TaillePage)
            .ToListAsync(cancellationToken);

        var dtos = dossiers.Select(d => new DossierListItemDto
        {
            Id = d.Id,
            Numero = d.Numero,
            Libelle = d.Libelle,
            DateOuverture = d.DateOuverture,
            Statut = d.Statut != null ? new StatutDto
            {
                Id = d.Statut.Id,
                Code = d.Statut.Code,
                Libelle = d.Statut.Libelle
            } : null,

        }).ToList();

        // Retou du ViewModel complet
        return new DossiersListVm
        {
            Dossiers = dtos,
            Page = request.Parameters.Page,
            TotalPage = (int)Math.Ceiling(totalCount / (double)request.Parameters.TaillePage),
            Recherche = request.Parameters.Recherche
        };
    }
}