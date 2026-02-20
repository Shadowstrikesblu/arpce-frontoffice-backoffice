using BackOffice.Application.Common.DTOs;
using BackOffice.Application.Common.Exceptions;
using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Dossiers.Queries.GetDossiersList;

public class GetDossiersListQueryHandler : IRequestHandler<GetDossiersListQuery, DossiersListVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetDossiersListQueryHandler(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<DossiersListVm> Handle(GetDossiersListQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Dossier> query = _context.Dossiers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Parameters.Status) && request.Parameters.Status.ToLower() != "all")
            query = query.Where(d => d.Statut.Code == request.Parameters.Status);

        if (request.Parameters.DateDebut.HasValue)
            query = query.Where(d => d.DateCreation >= new DateTimeOffset(request.Parameters.DateDebut.Value).ToUnixTimeMilliseconds());

        if (request.Parameters.DateFin.HasValue)
            query = query.Where(d => d.DateCreation <= new DateTimeOffset(request.Parameters.DateFin.Value.AddDays(1)).ToUnixTimeMilliseconds());

        if (!string.IsNullOrWhiteSpace(request.Parameters.Recherche))
        {
            var search = request.Parameters.Recherche.ToLower();
            query = query.Where(d => d.Numero.ToLower().Contains(search) || d.Libelle.ToLower().Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var dossiersPaged = await query
            .OrderByDescending(d => d.DateCreation)
            .Skip((request.Parameters.Page - 1) * request.Parameters.TaillePage)
            .Take(request.Parameters.TaillePage)
            .Include(d => d.Client)
            .Include(d => d.Statut)
            .Include(d => d.Demande).ThenInclude(dem => dem.Statut)
            .Include(d => d.Demande).ThenInclude(dem => dem.CategorieEquipement)
            .Include(d => d.Demande).ThenInclude(dem => dem.DocumentsDemandes)
            .Include(d => d.Demande).ThenInclude(dem => dem.Attestations)
            .ToListAsync(cancellationToken);

        var dossierDtos = dossiersPaged.Select(dossier => new DossierListItemDto
        {
            Id = dossier.Id,
            DateOuverture = dossier.DateOuverture.FromUnixTimeMilliseconds(),
            DateModification = dossier.DateModification?.FromUnixTimeMilliseconds(),
            Numero = dossier.Numero,
            Libelle = dossier.Libelle,
            Client = dossier.Client != null ? new ClientDto { Id = dossier.Client.Id, RaisonSociale = dossier.Client.RaisonSociale } : null,
            Statut = dossier.Statut != null ? new StatutDto { Id = dossier.Statut.Id, Code = dossier.Statut.Code, Libelle = dossier.Statut.Libelle } : null,

            Demandes = dossier.Demande != null ? new List<DemandeDto>
            {
                new DemandeDto
                {
                    Id = dossier.Demande.Id,
                    IdDossier = dossier.Id,
                    Equipement = dossier.Demande.Equipement,
                    Modele = dossier.Demande.Modele,
                    Marque = dossier.Demande.Marque,
                    Type = dossier.Demande.Type,
                    Statut = dossier.Demande.Statut != null ? new StatutDto { Id = dossier.Demande.Statut.Id, Code = dossier.Demande.Statut.Code, Libelle = dossier.Demande.Statut.Libelle } : null,
                    CategorieEquipement = dossier.Demande.CategorieEquipement != null ? new CategorieEquipementDto { Id = dossier.Demande.CategorieEquipement.Id, Code = dossier.Demande.CategorieEquipement.Code, Libelle = dossier.Demande.CategorieEquipement.Libelle } : null,
                    Documents = dossier.Demande.DocumentsDemandes.Select(doc => new DocumentDossierDto { Id = doc.Id, Nom = doc.Nom, FilePath = $"/api/demandes/demande/{doc.Id}/download" }).ToList()
                }
            } : new List<DemandeDto>(),

            Attestations = dossier.Demande != null
                ? dossier.Demande.Attestations.Select(att => new AttestationDto
                {
                    Id = att.Id,
                    DateDelivrance = att.DateDelivrance,
                    DateExpiration = att.DateExpiration,
                    FilePath = $"/api/documents/attestation/{att.Id}/download"
                }).ToList()
                : new List<AttestationDto>()
        }).ToList();

        return new DossiersListVm
        {
            Dossiers = dossierDtos,
            Page = request.Parameters.Page,
            PageTaille = request.Parameters.TaillePage,
            TotalPage = (int)Math.Ceiling(totalCount / (double)request.Parameters.TaillePage)
        };
    }
}