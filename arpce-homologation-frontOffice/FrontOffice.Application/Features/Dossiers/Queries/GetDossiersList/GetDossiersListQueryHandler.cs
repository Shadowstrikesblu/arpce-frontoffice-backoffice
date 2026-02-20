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
            .Include(d => d.Demande).ThenInclude(dem => dem.Statut) 
            .Include(d => d.Demande).ThenInclude(dem => dem.Attestations) 
            .AsQueryable();

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

            // On transforme l'unique demande en liste pour le DTO
            Demandes = dossier.Demande != null ? new List<DemandeDto>
            {
                new DemandeDto
                {
                    Id = dossier.Demande.Id,
                    Equipement = dossier.Demande.Equipement,
                    Statut = dossier.Demande.Statut != null ? new StatutDto
                    {
                        Id = dossier.Demande.Statut.Id,
                        Code = dossier.Demande.Statut.Code,
                        Libelle = dossier.Demande.Statut.Libelle
                    } : null
                }
            } : new List<DemandeDto>(),

            Devis = dossier.Devis?.Select(dev => new DevisDto { Id = dev.Id, PaiementOk = dev.PaiementOk }).ToList() ?? new(),

            Attestations = dossier.Demande != null ? dossier.Demande.Attestations
                .Where(att => att.Donnees != null && att.Donnees.Length > 0)
                .Select(att => new AttestationDto
                {
                    Id = att.Id,
                    DateDelivrance = att.DateDelivrance,
                    FilePath = $"/api/documents/certificat/{att.Id}/download"
                }).ToList() : new List<AttestationDto>()
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