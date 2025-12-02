using FrontOffice.Application.Common.DTOs;
using FrontOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FrontOffice.Application.Features.Dossiers.Queries.GetDossiersList;

/// <summary>
/// Gère la logique de la requête pour obtenir la liste paginée des dossiers.
/// </summary>
public class GetDossiersListQueryHandler : IRequestHandler<GetDossiersListQuery, DossiersListVm>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    /// Initialise une nouvelle instance du handler.
    /// </summary>
    public GetDossiersListQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Exécute la requête pour récupérer et traiter la liste des dossiers.
    /// </summary>
    public async Task<DossiersListVm> Handle(GetDossiersListQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("Utilisateur non authentifié.");
        }

        // Construire la requête de base (IQueryable)
        var query = _context.Dossiers
            .Where(d => d.IdClient == userId.Value)
            .Include(d => d.Statut) 
            .AsQueryable();

        // Applique le filtre de recherche (si fourni)
        if (!string.IsNullOrWhiteSpace(request.Parameters.Recherche))
        {
            var searchTerm = request.Parameters.Recherche.Trim().ToLower();
            query = query.Where(d =>
                d.Numero.ToLower().Contains(searchTerm) ||
                d.Libelle.ToLower().Contains(searchTerm) ||
                (d.Statut != null && d.Statut.Libelle.ToLower().Contains(searchTerm))
            );
        }

        // Applique le tri (si fourni)
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
                "statut" => isDescending
                    ? query.OrderByDescending(d => d.Statut.Libelle)
                    : query.OrderBy(d => d.Statut.Libelle),
                _ => query.OrderByDescending(d => d.DateOuverture) 
            };
        }
        else
        {
            // Applique un tri par défaut si aucun n'est spécifié, pour garantir un ordre constant.
            query = query.OrderByDescending(d => d.DateOuverture);
        }

        // Calcule le nombre total d'éléments (avant la pagination)
        var totalCount = await query.CountAsync(cancellationToken);

        // Applique la pagination
        var dossiersPaged = await query
            .Skip((request.Parameters.Page - 1) * request.Parameters.TaillePage)
            .Take(request.Parameters.TaillePage)
            .ToListAsync(cancellationToken);

        // Mappe les résultats vers le DTO
        var dossierDtos = dossiersPaged.Select(dossier => new DossierListItemDto
        {
            Id = dossier.Id,
            DateOuverture = dossier.DateOuverture,
            Numero = dossier.Numero,
            Libelle = dossier.Libelle,
            Statut = dossier.Statut != null ? new StatutDto
            {
                Id = dossier.Statut.Id,
                Code = dossier.Statut.Code,
                Libelle = dossier.Statut.Libelle
            } : null
        }).ToList();

        // Construction du ViewModel de réponse final
        var viewModel = new DossiersListVm
        {
            Dossiers = dossierDtos,
            Page = request.Parameters.Page,
            TotalPage = (int)Math.Ceiling(totalCount / (double)request.Parameters.TaillePage),
            Recherche = request.Parameters.Recherche
        };

        return viewModel;
    }
}