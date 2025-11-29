using BackOffice.Application.Common.DTOs;
using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Dossiers.Queries.GetDossiersList;

/// <summary>
/// Gère la logique de la requête pour obtenir la liste paginée et filtrée de tous les dossiers
/// pour le Back Office.
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
        // Construire la requête de base (IQueryable)
        var query = _context.Dossiers.AsNoTracking();

        // Appliquer les filtres
        if (!string.IsNullOrWhiteSpace(request.Parameters.Recherche))
        {
            var searchTerm = request.Parameters.Recherche.Trim().ToLower();
            // La recherche s'applique sur le numéro, le libellé du dossier, ou la raison sociale du client.
            query = query.Where(d =>
                d.Numero.ToLower().Contains(searchTerm) ||
                d.Libelle.ToLower().Contains(searchTerm) ||
                (d.Client != null && d.Client.RaisonSociale.ToLower().Contains(searchTerm))
            );
        }

        // Filtre par statut
        if (!string.IsNullOrWhiteSpace(request.Parameters.Status))
        {
            var statusTerm = request.Parameters.Status.Trim();
            // Le filtre s'applique sur le code du statut.
            query = query.Where(d => d.Statut.Code == statusTerm);
        }

        // Applique le tri
        bool isDescending = request.Parameters.Ordre?.ToLower() == "desc";

        switch (request.Parameters.TrierPar?.ToLower())
        {
            case "date_creation":
                query = isDescending ? query.OrderByDescending(d => d.DateCreation) : query.OrderBy(d => d.DateCreation);
                break;
            case "date-update":
                query = isDescending ? query.OrderByDescending(d => d.DateModification) : query.OrderBy(d => d.DateModification);
                break;
            case "libelle":
                query = isDescending ? query.OrderByDescending(d => d.Libelle) : query.OrderBy(d => d.Libelle);
                break;
            default:
                // Tri par défaut : les plus récents en premier
                query = query.OrderByDescending(d => d.DateCreation);
                break;
        }

        // Calcule le nombre total d'éléments après filtrage
        var totalCount = await query.CountAsync(cancellationToken);
        if (totalCount == 0)
        {
            // Si aucun résultat, on retourne une réponse vide immédiatement pour éviter des calculs inutiles.
            return new DossiersListVm { Page = request.Parameters.Page, TotalPage = 0, Dossiers = new() };
        }

        // Applique la pagination et exécuter la requête
        var dossiersPaged = await query
            .Include(d => d.Client)   
            .Include(d => d.Statut)   
            .Include(d => d.Demandes) 
            .Skip((request.Parameters.Page - 1) * request.Parameters.TaillePage)
            .Take(request.Parameters.TaillePage)
            .ToListAsync(cancellationToken);

        // Mappe les résultats vers les DTOs
        var dossierDtos = dossiersPaged.Select(dossier => new DossierListItemDto
        {
            Id = dossier.Id,
            DateOuverture = dossier.DateOuverture,
            Numero = dossier.Numero,
            Libelle = dossier.Libelle,
            Client = dossier.Client != null ? new ClientDto
            {
                Id = dossier.Client.Id,
                RaisonSociale = dossier.Client.RaisonSociale
            } : null,
            Statut = dossier.Statut != null ? new StatutDto
            {
                Id = dossier.Statut.Id,
                Code = dossier.Statut.Code,
                Libelle = dossier.Statut.Libelle
            } : null,
            Demandes = dossier.Demandes.Select(demande => new DemandeDto
            {
                Id = demande.Id,
                Equipement = demande.Equipement,
                Modele = demande.Modele,
                Marque = demande.Marque
            }).ToList()
        }).ToList();

        // Construire le ViewModel de réponse final
        var viewModel = new DossiersListVm
        {
            Dossiers = dossierDtos,
            Page = request.Parameters.Page,
            PageTaille = request.Parameters.TaillePage,
            TotalPage = (int)Math.Ceiling(totalCount / (double)request.Parameters.TaillePage)
        };

        return viewModel;
    }
}