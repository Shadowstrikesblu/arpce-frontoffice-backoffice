using BackOffice.Application.Common.DTOs.Documents;
using BackOffice.Application.Common.Interfaces;
using BackOffice.Application.Features.Documents.Queries.GetFacturesList;
using BackOffice.Domain.Entities;
using BackOffice.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Documents.Queries.GetPaiementsList;

public class GetPaiementsListQueryHandler : IRequestHandler<GetPaiementsListQuery, DocumentListVm>
{
    private readonly IApplicationDbContext _context;

    public GetPaiementsListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DocumentListVm> Handle(GetPaiementsListQuery request, CancellationToken cancellationToken)
    {
        // Statut cible : Dossier payé (DossierPayer) - Assurez-vous que cet enum existe
        var targetStatus = StatutDossierEnum.DossierPayer.ToString();

        IQueryable<Dossier> query = _context.Dossiers.AsNoTracking();

        query = query.Where(d => d.Statut.Code == targetStatus);

        if (!string.IsNullOrWhiteSpace(request.Recherche))
        {
            var term = request.Recherche.ToLower();
            query = query.Where(d => d.Numero.ToLower().Contains(term) || d.Libelle.ToLower().Contains(term));
        }

        query = request.Ordre?.ToLower() == "asc"
            ? query.OrderBy(d => d.DateCreation)
            : query.OrderByDescending(d => d.DateCreation);

        var total = await query.CountAsync(cancellationToken);

        var dossiers = await query
            .Skip((request.Page - 1) * request.PageTaille)
            .Take(request.PageTaille)
            .Include(d => d.Devis)
            .ToListAsync(cancellationToken);

        var documents = dossiers.Select(d =>
        {
            var devis = d.Devis.FirstOrDefault();
            return new DocumentItemDto
            {
                Nom = $"RecuPaiement_{d.Numero}",
                Type = 2,
                Extension = "pdf",
                Url = $"/api/documents/paiement/{d.Id}",
                Dossier = new DossierInfoInDocDto
                {
                    Numero = d.Numero,
                    Libelle = d.Libelle,
                    Devis = devis != null ? new DevisInfoInDocDto
                    {
                        Date = devis.Date,
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