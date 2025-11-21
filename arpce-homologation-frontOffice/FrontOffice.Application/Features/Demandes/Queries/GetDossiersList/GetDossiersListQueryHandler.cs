using FrontOffice.Application.Common.DTOs;
using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FrontOffice.Application.Features.Dossiers.Queries.GetDossiersList;

/// <summary>
/// Gère la logique de la requête pour obtenir la liste paginée des dossiers du client.
/// Cette version inclut la liste des demandes (équipements) pour chaque dossier.
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
            throw new UnauthorizedAccessException("Accès non autorisé. L'authentification est requise.");
        }

        // Construire la requête de base (IQueryable)
        IQueryable<Dossier> query = _context.Dossiers.AsQueryable();

        // On filtre d'abord par client.
        query = query.Where(d => d.IdClient == userId.Value);

        // Applique le filtre de recherche
        if (!string.IsNullOrWhiteSpace(request.Parameters.Recherche))
        {
            var searchTerm = request.Parameters.Recherche.Trim().ToLower();
            query = query.Where(d =>
                d.Numero.ToLower().Contains(searchTerm) ||
                d.Libelle.ToLower().Contains(searchTerm) ||
                (d.Statut != null && d.Statut.Libelle.ToLower().Contains(searchTerm))
            );
        }

        // Applique le tri
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
            query = query.OrderByDescending(d => d.DateOuverture);
        }

        // Étape 4 : Calculer le nombre total d'éléments
        var totalCount = await query.CountAsync(cancellationToken);

        // Étape 5 : Appliquer la pagination et charger les données associées
        // On applique les Include() juste avant l'exécution finale (ToListAsync).
        var dossiersPaged = await query
            .Include(d => d.Statut)
            .Include(d => d.Demandes)
            .Skip((request.Parameters.Page - 1) * request.Parameters.TaillePage)
            .Take(request.Parameters.TaillePage)
            .AsNoTracking() // Appliqué ici pour optimiser
            .ToListAsync(cancellationToken);

        // Étape 6 : Mapper les résultats vers les DTOs
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
            } : null,
            Demandes = dossier.Demandes.Select(demande => new DemandeDto
            {
                Id = demande.Id,
                IdDossier = demande.IdDossier,
                Equipement = demande.Equipement,
                Modele = demande.Modele,
                Marque = demande.Marque
            }).ToList()
        }).ToList();

        // Étape 7 : Construire le ViewModel de réponse
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