using BackOffice.Application.Common.DTOs.Documents;
using BackOffice.Application.Common.Exceptions;
using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities; 
using BackOffice.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Documents.Queries.GetFacturesList;

public class GetFacturesListQueryHandler : IRequestHandler<GetFacturesListQuery, DocumentListVm>
{
    private readonly IApplicationDbContext _context;

    public GetFacturesListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DocumentListVm> Handle(GetFacturesListQuery request, CancellationToken cancellationToken)
    {
        var targetStatus = StatutDossierEnum.DevisEmit.ToString();

        // On part d'un IQueryable<Dossier> simple
        IQueryable<Dossier> query = _context.Dossiers.AsNoTracking();

        // 1. Filtrage par statut
        query = query.Where(d => d.Statut.Code == targetStatus);

        // 2. Filtrage par recherche
        if (!string.IsNullOrWhiteSpace(request.Recherche))
        {
            var term = request.Recherche.ToLower();
            query = query.Where(d => d.Numero.ToLower().Contains(term) || d.Libelle.ToLower().Contains(term));
        }

        // 3. Tri
        query = request.Ordre?.ToLower() == "asc"
            ? query.OrderBy(d => d.DateCreation)
            : query.OrderByDescending(d => d.DateCreation);

        // 4. Pagination et Chargement des données (Include à la fin)
        var total = await query.CountAsync(cancellationToken);

        var dossiers = await query
            .Skip((request.Page - 1) * request.PageTaille)
            .Take(request.PageTaille)
            .Include(d => d.Devis) // Chargement des devis ici
            .ToListAsync(cancellationToken);

        // 5. Mapping
        var documents = dossiers.Select(d =>
        {
            var devis = d.Devis.FirstOrDefault();
            return new DocumentItemDto
            {
                Nom = $"Facture_{d.Numero}",
                Type = 1,
                Extension = "pdf",
                Url = $"/api/documents/facture/{d.Id}",
                Dossier = new DossierInfoInDocDto
                {
                    Numero = d.Numero,
                    Libelle = d.Libelle,
                    Devis = devis != null ? new DevisInfoInDocDto
                    {
                        Date = devis.Date.FromUnixTimeMilliseconds(),
                        MontantEtude = devis.MontantEtude,
                        MontantHomologation = devis.MontantHomologation,
                        MontantControle = devis.MontantControle,
                        PaiementOk = devis.PaiementOk,
                        PaiementMobileId = devis.PaiementMobileId
                    } : null
                }
            };
        }).ToList();

        return new DocumentListVm
        {
            Page = request.Page,
            PageTaille = request.PageTaille,
            Document = documents
        };
    }
}