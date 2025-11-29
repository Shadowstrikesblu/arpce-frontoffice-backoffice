using BackOffice.Application.Common.DTOs;
using BackOffice.Application.Common.Interfaces;
using BackOffice.Application.Features.Dossiers.Queries.GetDossiersList; 
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Dossiers.Queries.GetMyDossiers;

/// <summary>
/// Gère la logique de la requête pour obtenir la liste des dossiers spécifiquement assignés à l'agent connecté.
/// </summary>
public class GetMyDossiersQueryHandler : IRequestHandler<GetMyDossiersQuery, DossiersListVm>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    /// Initialise une nouvelle instance du handler.
    /// </summary>
    public GetMyDossiersQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Exécute la requête pour récupérer la liste des dossiers assignés.
    /// </summary>
    public async Task<DossiersListVm> Handle(GetMyDossiersQuery request, CancellationToken cancellationToken)
    {
        var agentId = _currentUserService.UserId;
        if (!agentId.HasValue)
        {
            // L'utilisateur doit être un agent authentifié pour avoir des dossiers assignés.
            throw new UnauthorizedAccessException("Accès non autorisé. L'authentification de l'agent est requise.");
        }

        var query = _context.Dossiers.AsNoTracking();

        // On ne retourne que les dossiers où 'IdAgentInstructeur' correspond à l'ID de l'agent connecté.
        query = query.Where(d => d.IdAgentInstructeur == agentId.Value);

        // Applique les filtres de recherche et de statut 
        if (!string.IsNullOrWhiteSpace(request.Parameters.Recherche))
        {
            var searchTerm = request.Parameters.Recherche.Trim().ToLower();
            query = query.Where(d =>
                d.Numero.ToLower().Contains(searchTerm) ||
                d.Libelle.ToLower().Contains(searchTerm) ||
                (d.Client != null && d.Client.RaisonSociale.ToLower().Contains(searchTerm))
            );
        }

        if (!string.IsNullOrWhiteSpace(request.Parameters.Status))
        {
            var statusTerm = request.Parameters.Status.Trim();
            query = query.Where(d => d.Statut.Code == statusTerm);
        }

        // Applique le tri 
        bool isDescending = request.Parameters.Ordre?.ToLower() == "desc";
        query = request.Parameters.TrierPar?.ToLower() switch
        {
            "date_creation" => isDescending ? query.OrderByDescending(d => d.DateCreation) : query.OrderBy(d => d.DateCreation),
            "date-update" => isDescending ? query.OrderByDescending(d => d.DateModification) : query.OrderBy(d => d.DateModification),
            "libelle" => isDescending ? query.OrderByDescending(d => d.Libelle) : query.OrderBy(d => d.Libelle),
            _ => query.OrderByDescending(d => d.DateCreation)
        };

        // Calcule le nombre total d'éléments après tous les filtres
        var totalCount = await query.CountAsync(cancellationToken);
        if (totalCount == 0)
        {
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

        // Mapper les résultats vers les DTOs 
        var dossierDtos = dossiersPaged.Select(dossier => new DossierListItemDto
        {
            Id = dossier.Id,
            DateOuverture = dossier.DateOuverture,
            Numero = dossier.Numero,
            Libelle = dossier.Libelle,
            Client = dossier.Client != null ? new ClientDto { Id = dossier.Client.Id, RaisonSociale = dossier.Client.RaisonSociale } : null,
            Statut = dossier.Statut != null ? new StatutDto { Id = dossier.Statut.Id, Code = dossier.Statut.Code, Libelle = dossier.Statut.Libelle } : null,
            Demandes = dossier.Demandes.Select(demande => new DemandeDto { Id = demande.Id, Equipement = demande.Equipement, Modele = demande.Modele, Marque = demande.Marque }).ToList()
        }).ToList();

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