using BackOffice.Application.Common;
using BackOffice.Application.Common.DTOs;
using BackOffice.Application.Common.Exceptions;
using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Dossiers.Queries.GetDossiersList;

public class GetDossiersListQueryHandler : IRequestHandler<GetDossiersListQuery, DossiersListVm>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetDossiersListQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _currentUserService = currentUserService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<DossiersListVm> Handle(GetDossiersListQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Dossier> query = _context.Dossiers.AsNoTracking();

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

        var totalCount = await query.CountAsync(cancellationToken);
        if (totalCount == 0)
        {
            return new DossiersListVm { Page = request.Parameters.Page, TotalPage = 0, PageTaille = request.Parameters.TaillePage, Dossiers = new() };
        }

        var dossiersPaged = await query
            .Skip((request.Parameters.Page - 1) * request.Parameters.TaillePage)
            .Take(request.Parameters.TaillePage)
            .Include(d => d.Client)
            .Include(d => d.Statut)
            .Include(d => d.DocumentsDossiers)
            .Include(d => d.Demandes).ThenInclude(dem => dem.DocumentsDemandes)
            .Include(d => d.Demandes).ThenInclude(dem => dem.Attestations)
            .ToListAsync(cancellationToken);

        var requestContext = _httpContextAccessor.HttpContext!.Request;
        var baseUrl = $"{requestContext.Scheme}://{requestContext.Host}";

        var dossierDtos = dossiersPaged.Select(dossier => new DossierListItemDto
        {
            Id = dossier.Id,
            DateOuverture = dossier.DateOuverture.FromUnixTimeMilliseconds(),
            Numero = dossier.Numero,
            Libelle = dossier.Libelle,
            Client = dossier.Client != null ? new ClientDto { Id = dossier.Client.Id, RaisonSociale = dossier.Client.RaisonSociale } : null,
            Statut = dossier.Statut != null ? new StatutDto { Id = dossier.Statut.Id, Code = dossier.Statut.Code, Libelle = dossier.Statut.Libelle } : null,

            Demandes = dossier.Demandes.Select(demande => new DemandeDto
            {
                Id = demande.Id,
                Equipement = demande.Equipement,
                Modele = demande.Modele,
                Marque = demande.Marque,
                Documents = demande.DocumentsDemandes.Select(doc => new DocumentDossierDto
                {
                    Id = doc.Id,
                    Nom = doc.Nom,
                    Extension = doc.Extension,
                    Type = null,
                    FilePath = $"/api/demandes/demande/{doc.Id}/download"
                }).ToList()
            }).ToList(),

            Documents = dossier.DocumentsDossiers.Select(doc => new DocumentDossierDto
            {
                Id = doc.Id,
                Nom = doc.Nom,
                Type = doc.Type,
                Extension = doc.Extension,
                FilePath = $"/api/demandes/dossier/{doc.Id}/download"
            }).ToList(),

            Attestations = dossier.Demandes.SelectMany(dem => dem.Attestations).Select(att => new AttestationDto
            {
                Id = att.Id,
                DateDelivrance = att.DateDelivrance,
                DateExpiration = att.DateExpiration,
                FilePath = $"/api/documents/attestation/{att.Id}"
            }).ToList()
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