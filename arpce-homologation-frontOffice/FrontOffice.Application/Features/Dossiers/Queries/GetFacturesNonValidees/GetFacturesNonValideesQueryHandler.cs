using FrontOffice.Application.Common.DTOs;
using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Application.Features.Dossiers.Queries.GetDossiersList;
using FrontOffice.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FrontOffice.Application.Features.Dossiers.Queries.GetFacturesNonValidees;

public class GetFacturesNonValideesQueryHandler : IRequestHandler<GetFacturesNonValideesQuery, DossiersListVm>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetFacturesNonValideesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<DossiersListVm> Handle(GetFacturesNonValideesQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue) throw new UnauthorizedAccessException("Utilisateur non authentifié.");

        // Statut cible : En attente de paiement (Facture disponible)
        // Le code 'DevisPaiement' correspond à "Approuvé, en attente de paiement".
        var statutCible = StatutDossierEnum.DevisPaiement.ToString();
        // Code du type de document "Facture" (à définir, supposons 2)
        byte typeFacture = 2;

        // 1. Requête de base
        var query = _context.Dossiers.AsNoTracking()
            .Where(d => d.IdClient == userId.Value && d.Statut.Code == statutCible);

        // 2. Recherche
        if (!string.IsNullOrWhiteSpace(request.Parameters.Recherche))
        {
            var term = request.Parameters.Recherche.Trim().ToLower();
            query = query.Where(d => d.Numero.ToLower().Contains(term) || d.Libelle.ToLower().Contains(term));
        }

        // 3. Tri
        if (!string.IsNullOrWhiteSpace(request.Parameters.TrierPar))
        {
            bool isDescending = request.Parameters.Ordre?.ToLower() == "desc";
            query = request.Parameters.TrierPar.ToLower() switch
            {
                "date_creation" or "dateouverture" => isDescending ? query.OrderByDescending(d => d.DateOuverture) : query.OrderBy(d => d.DateOuverture),
                "libelle" => isDescending ? query.OrderByDescending(d => d.Libelle) : query.OrderBy(d => d.Libelle),
                _ => query.OrderByDescending(d => d.DateOuverture)
            };
        }
        else
        {
            query = query.OrderByDescending(d => d.DateOuverture);
        }

        // 4. Pagination et Chargement
        var totalCount = await query.CountAsync(cancellationToken);

        var dossiers = await query
            .Include(d => d.Statut)
            // On inclut les documents, et on pourra filtrer côté mémoire si besoin, 
            // ou on filtre dans le Select si on veut être précis.
            .Include(d => d.DocumentsDossiers)
            .Skip((request.Parameters.Page - 1) * request.Parameters.TaillePage)
            .Take(request.Parameters.TaillePage)
            .ToListAsync(cancellationToken);

        // 5. Mapping
        var dtos = dossiers.Select(d => new DossierListItemDto
        {
            Id = d.Id,
            Numero = d.Numero,
            Libelle = d.Libelle,
            DateOuverture = d.DateOuverture,
            Statut = d.Statut != null ? new StatutDto { Id = d.Statut.Id, Code = d.Statut.Code, Libelle = d.Statut.Libelle } : null,

            // On ne garde que les documents de type "Facture" pour cet affichage
            // Assurez-vous que 'typeFacture' correspond à votre logique métier
            Documents = d.DocumentsDossiers
                .Where(doc => doc.Type == typeFacture)
                .Select(doc => new DocumentDossierDto
                {
                    Id = doc.Id,
                    Nom = doc.Nom,
                    Type = doc.Type,
                    Extension = doc.Extension,
                    FilePath = doc.FilePath
                }).ToList()
        }).ToList();

        return new DossiersListVm
        {
            Dossiers = dtos,
            Page = request.Parameters.Page,
            TotalPage = (int)Math.Ceiling(totalCount / (double)request.Parameters.TaillePage),
            Recherche = request.Parameters.Recherche
        };
    }
}