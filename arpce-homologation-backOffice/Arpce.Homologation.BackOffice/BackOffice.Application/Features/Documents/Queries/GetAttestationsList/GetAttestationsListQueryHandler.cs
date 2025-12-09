using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BackOffice.Application.Common.Exceptions;
using BackOffice.Application.Common;

namespace BackOffice.Application.Features.Documents.Queries.GetAttestationsList;

public class GetAttestationsListQueryHandler : IRequestHandler<GetAttestationsListQuery, AttestationListVm>
{
    private readonly IApplicationDbContext _context;

    public GetAttestationsListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AttestationListVm> Handle(GetAttestationsListQuery request, CancellationToken cancellationToken)
    {
        var targetStatus = StatutDossierEnum.DossierSigne.ToString();

        var query = _context.Attestations.AsNoTracking()
            .Include(a => a.Demande)
            .ThenInclude(dem => dem.Dossier)
            .Where(a => a.Demande.Dossier.Statut.Code == targetStatus);

        if (!string.IsNullOrWhiteSpace(request.Recherche))
        {
            var term = request.Recherche.ToLower();
            query = query.Where(a => a.Demande.Dossier.Numero.ToLower().Contains(term) || a.Demande.Dossier.Libelle.ToLower().Contains(term));
        }

        query = request.Ordre?.ToLower() == "asc"
            ? query.OrderBy(a => a.DateDelivrance)
            : query.OrderByDescending(a => a.DateDelivrance);

        var total = await query.CountAsync(cancellationToken);
        var attestations = await query
            .Skip((request.Page - 1) * request.PageTaille)
            .Take(request.PageTaille)
            .ToListAsync(cancellationToken);

        var items = attestations.Select(a => new AttestationItemDto
        {
            
            DateDelivrance = a.DateDelivrance.FromUnixTimeMilliseconds(),
            DateExpiration = a.DateExpiration.FromUnixTimeMilliseconds(),
            Extension = a.Extension,
            Url = $"/api/documents/attestation/{a.Id}",
            Dossier = new DossierSimpleDto
            {
                Numero = a.Demande.Dossier.Numero,
                Libelle = a.Demande.Dossier.Libelle,
                Url = $"/api/dossiers/{a.Demande.Dossier.Id}"
            }
        }).ToList();

        return new AttestationListVm
        {
            Page = request.Page,
            PageTaille = request.PageTaille,
            Attestation = items
        };
    }
}