using FrontOffice.Application.Common.DTOs;
using FrontOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FrontOffice.Application.Features.Dossiers.Queries.GetDossiersList;

public class GetDossiersListQueryHandler : IRequestHandler<GetDossiersListQuery, DossiersListVm>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetDossiersListQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<DossiersListVm> Handle(GetDossiersListQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue) throw new UnauthorizedAccessException("Utilisateur non authentifié.");

        var query = _context.Dossiers
            .AsNoTracking()
            .Where(d => d.IdClient == userId.Value)
            .Include(d => d.Statut)
            .Include(d => d.Demandes).ThenInclude(dem => dem.Statut)
            .Include(d => d.Demandes).ThenInclude(dem => dem.Attestations) // Inclusion des attestations
            .AsQueryable();

        // Filtre recherche
        if (!string.IsNullOrWhiteSpace(request.Parameters.Recherche))
        {
            var search = request.Parameters.Recherche.ToLower();
            query = query.Where(d => d.Numero.ToLower().Contains(search) || d.Libelle.ToLower().Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var dossiersPaged = await query
            .OrderByDescending(d => d.DateOuverture)
            .Skip((request.Parameters.Page - 1) * request.Parameters.TaillePage)
            .Take(request.Parameters.TaillePage)
            .ToListAsync(cancellationToken);

        var dossierDtos = dossiersPaged.Select(dossier => new DossierListItemDto
        {
            Id = dossier.Id,
            DateOuverture = dossier.DateOuverture,
            Numero = dossier.Numero,
            Libelle = dossier.Libelle,
            Statut = dossier.Statut != null ? new StatutDto { Id = dossier.Statut.Id, Code = dossier.Statut.Code, Libelle = dossier.Statut.Libelle } : null,

            Demandes = dossier.Demandes.Select(dem => new DemandeDto
            {
                Id = dem.Id,
                Equipement = dem.Equipement,
                Statut = dem.Statut != null ? new StatutDto
                {
                    Id = dem.Statut.Id,
                    Code = dem.Statut.Code,
                    Libelle = dem.Statut.Libelle
                } : null
            }).ToList(),

            Devis = dossier.Devis?.Select(dev => new DevisDto { Id = dev.Id, PaiementOk = dev.PaiementOk }).ToList() ?? new(),

            // Ajout du mapping des attestations (uniquement celles signées)
            Attestations = dossier.Demandes.SelectMany(dem => dem.Attestations)
                .Where(att => att.Donnees != null && att.Donnees.Length > 0)
                .Select(att => new AttestationDto
                {
                    Id = att.Id,
                    DateDelivrance = att.DateDelivrance,
                    FilePath = $"/api/documents/certificat/{att.Id}/download"
                }).ToList()
        }).ToList();

        return new DossiersListVm
        {
            Dossiers = dossierDtos,
            Page = request.Parameters.Page,
            TotalPage = (int)Math.Ceiling(totalCount / (double)request.Parameters.TaillePage),
            Recherche = request.Parameters.Recherche
        };
    }
}