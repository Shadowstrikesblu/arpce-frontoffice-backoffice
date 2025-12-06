using FrontOffice.Application.Common.DTOs;
using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FrontOffice.Application.Features.Dossiers.Queries.GetDossiersFullList;

public class GetDossiersFullListQueryHandler : IRequestHandler<GetDossiersFullListQuery, DossiersFullListVm>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetDossiersFullListQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<DossiersFullListVm> Handle(GetDossiersFullListQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue) throw new UnauthorizedAccessException("Utilisateur non authentifié.");

        // Requête de base avec filtre client
        IQueryable<Dossier> query = _context.Dossiers
            .AsNoTracking()
            .Where(d => d.IdClient == userId.Value);

        // Filtres
        if (!string.IsNullOrWhiteSpace(request.Parameters.Recherche))
        {
            var term = request.Parameters.Recherche.Trim().ToLower();
            query = query.Where(d => d.Numero.ToLower().Contains(term) || d.Libelle.ToLower().Contains(term));
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

        var totalCount = await query.CountAsync(cancellationToken);

        // Pagination et chargement COMPLET des données
        var dossiers = await query
            .OrderByDescending(d => d.DateOuverture) // Tri par défaut
            .Skip((request.Parameters.Page - 1) * request.Parameters.TaillePage)
            .Take(request.Parameters.TaillePage)
            // Chargement de toutes les relations
            .Include(d => d.Statut)
            .Include(d => d.ModeReglement)
            .Include(d => d.Commentaires)
            .Include(d => d.Devis)
            .Include(d => d.DocumentsDossiers)
            .Include(d => d.Demandes).ThenInclude(dem => dem.Attestations)
            .Include(d => d.Demandes).ThenInclude(dem => dem.CategorieEquipement)
            .Include(d => d.Demandes).ThenInclude(dem => dem.MotifRejet)
            .Include(d => d.Demandes).ThenInclude(dem => dem.Proposition)
            .ToListAsync(cancellationToken);

        // Mapping complet vers les DTOs
        var dtos = dossiers.Select(d => new DossierFullListItemDto
        {
            Id = d.Id,
            Numero = d.Numero,
            Libelle = d.Libelle,
            DateOuverture = d.DateOuverture,
            NbDemandes = d.Demandes.Count,

            Statut = d.Statut != null ? new StatutDto { Id = d.Statut.Id, Code = d.Statut.Code, Libelle = d.Statut.Libelle } : null,
            ModeReglement = d.ModeReglement != null ? new ModeReglementDto { Id = d.ModeReglement.Id, Code = d.ModeReglement.Code, Libelle = d.ModeReglement.Libelle } : null,

            Commentaires = d.Commentaires.Select(com => new CommentaireDto { Id = com.Id, DateCommentaire = com.DateCommentaire, CommentaireTexte = com.CommentaireTexte }).ToList(),
            Devis = d.Devis.Select(dev => new DevisDto { Id = dev.Id, Date = dev.Date, MontantEtude = dev.MontantEtude }).ToList(),
            Documents = d.DocumentsDossiers.Select(doc => new DocumentDossierDto { Id = doc.Id, Nom = doc.Nom, FilePath = doc.FilePath }).ToList(),

            Demandes = d.Demandes.Select(dem => new DemandeDto
            {
                Id = dem.Id,
                Equipement = dem.Equipement,
                Modele = dem.Modele,
                Marque = dem.Marque,
                Fabricant = dem.Fabricant,
                QuantiteEquipements = dem.QuantiteEquipements,
                PrixUnitaire = dem.PrixUnitaire,
                Remise = dem.Remise,
                EstHomologable = dem.EstHomologable,
                CategorieEquipement = dem.CategorieEquipement != null ? new CategorieEquipementDto { Id = dem.CategorieEquipement.Id, Code = dem.CategorieEquipement.Code, Libelle = dem.CategorieEquipement.Libelle } : null,
                MotifRejet = dem.MotifRejet != null ? new MotifRejetDto { Id = dem.MotifRejet.Id, Code = dem.MotifRejet.Code, Libelle = dem.MotifRejet.Libelle } : null,
                Proposition = dem.Proposition != null ? new PropositionDto { Id = dem.Proposition.Id, Code = dem.Proposition.Code, Libelle = dem.Proposition.Libelle } : null
            }).ToList(),

            Attestations = d.Demandes.SelectMany(dem => dem.Attestations).Select(att => new AttestationDto { Id = att.Id, DateDelivrance = att.DateDelivrance, DateExpiration = att.DateExpiration }).ToList()
        }).ToList();

        return new DossiersFullListVm
        {
            Page = request.Parameters.Page,
            PageTaille = request.Parameters.TaillePage,
            TotalPage = (int)Math.Ceiling(totalCount / (double)request.Parameters.TaillePage),
            Dossiers = dtos
        };
    }
}