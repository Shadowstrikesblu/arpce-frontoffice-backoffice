using BackOffice.Application.Common.DTOs;
using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Dossiers.Queries.GetDossiersList;

/// <summary>
/// Gère la logique de la requête pour obtenir la liste paginée et filtrée de tous les dossiers
/// pour le Back Office.
/// </summary>
public class GetDossiersListQueryHandler : IRequestHandler<GetDossiersListQuery, DossiersListVm>
{
    // Déclaration unique du contexte
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
        // Création de la requête de base
        IQueryable<Dossier> query = _context.Dossiers.AsNoTracking();

        // Application des filtres
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

        // Application du tri
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
                query = query.OrderByDescending(d => d.DateCreation);
                break;
        }

        // Comptage
        var totalCount = await query.CountAsync(cancellationToken);
        if (totalCount == 0)
        {
            return new DossiersListVm { Page = request.Parameters.Page, TotalPage = 0, PageTaille = request.Parameters.TaillePage, Dossiers = new() };
        }

        // Pagination et Chargement des données
        var dossiersPaged = await query
            .Include(d => d.Client)
            .Include(d => d.Statut)
            .Include(d => d.Demandes)
            .Skip((request.Parameters.Page - 1) * request.Parameters.TaillePage)
            .Take(request.Parameters.TaillePage)
            .ToListAsync(cancellationToken);

        // Mapping
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

        // Retourne du ViewModel
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