using FrontOffice.Application.Common.DTOs;
using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        IQueryable<Dossier> query = _context.Dossiers
            .AsNoTracking()
            .Where(d => d.IdClient == userId.Value);

        if (!string.IsNullOrWhiteSpace(request.Parameters.Recherche))
        {
            var term = request.Parameters.Recherche.Trim().ToLower();
            query = query.Where(d => d.Numero.ToLower().Contains(term) || d.Libelle.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var dossiers = await query
            .OrderByDescending(d => d.DateOuverture)
            .Skip((request.Parameters.Page - 1) * request.Parameters.TaillePage)
            .Take(request.Parameters.TaillePage)
            .Include(d => d.Statut)
            .Include(d => d.Devis)
            .Include(d => d.DocumentsDossiers)
            .Include(d => d.Demande).ThenInclude(dem => dem.Statut)
            .Include(d => d.Demande).ThenInclude(dem => dem.CategorieEquipement)
            .Include(d => d.Demande).ThenInclude(dem => dem.Beneficiaire)
            .Include(d => d.Demande).ThenInclude(dem => dem.DocumentsDemandes)
            .ToListAsync(cancellationToken);

        var dtos = dossiers.Select(d => new DossierFullListItemDto
        {
            Id = d.Id,
            Numero = d.Numero,
            Libelle = d.Libelle,
            DateOuverture = d.DateOuverture,
            //NbDemandes = d.Demande != null ? 1 : 0,

            Statut = d.Statut != null ? new StatutDto { Id = d.Statut.Id, Code = d.Statut.Code, Libelle = d.Statut.Libelle } : null,

            Devis = d.Devis.Select(dev => new DevisDto
            {
                Id = dev.Id,
                Date = dev.Date,
                MontantEtude = dev.MontantEtude,
                MontantHomologation = dev.MontantHomologation,
                MontantControle = dev.MontantControle,
                MontantPenalite = dev.MontantPenalite,
                MontantTotal = dev.MontantTotal,
                PaiementOk = dev.PaiementOk,
                FilePath = $"/api/devis/{dev.Id}/download"
            }).ToList(),

            Demandes = d.Demande != null ? new List<DemandeDto>
            {
                new DemandeDto
                {
                    Id = d.Demande.Id,
                IdDossier = d.Id,
                Equipement = d.Demande.Equipement,
                Modele = d.Demande.Modele,
                Marque = d.Demande.Marque,
                Fabricant = d.Demande.Fabricant,
                Type = d.Demande.Type,
                Description = d.Demande.Description,
                QuantiteEquipements = d.Demande.QuantiteEquipements,
                PrixUnitaire = d.Demande.PrixUnitaire,
                EstHomologable = d.Demande.EstHomologable,
                    CategorieEquipement = d.Demande.CategorieEquipement != null ? new CategorieEquipementDto
                    {
                        Id = d.Demande.CategorieEquipement.Id,
                        Code = d.Demande.CategorieEquipement.Code,
                        Libelle = d.Demande.CategorieEquipement.Libelle,
                        TypeEquipement = d.Demande.CategorieEquipement.TypeEquipement,
                        TypeClient = d.Demande.CategorieEquipement.TypeClient,
                        FraisEtude = d.Demande.CategorieEquipement.FraisEtude,
                        FraisHomologation = d.Demande.CategorieEquipement.FraisHomologation,
                        FraisControle = d.Demande.CategorieEquipement.FraisControle,
                        ModeCalcul = d.Demande.CategorieEquipement.ModeCalcul,
                        BlockSize = d.Demande.CategorieEquipement.BlockSize,
                        ReferenceLoiFinance = d.Demande.CategorieEquipement.ReferenceLoiFinance,
                        Remarques = d.Demande.CategorieEquipement.Remarques,
                        QtyMin = d.Demande.CategorieEquipement.QtyMin,
                        QtyMax = d.Demande.CategorieEquipement.QtyMax
                    } : null,
                    Beneficiaire = d.Demande.Beneficiaire != null ? new BeneficiaireDto
                    {
                        Id = d.Demande.Beneficiaire.Id,
                        Nom = d.Demande.Beneficiaire.Nom,
                        Email = d.Demande.Beneficiaire.Email,
                        Adresse = d.Demande.Beneficiaire.Adresse,
                        Telephone = d.Demande.Beneficiaire.Telephone,
                        Type = d.Demande.Beneficiaire.Type,
                        //LettreDocumentPath = d.Demande.Beneficiaire.LettreDocumentPath
                    } : null
                }
            } : new List<DemandeDto>(),

            Documents = d.DocumentsDossiers.Select(doc => new DocumentDossierDto { Id = doc.Id, Nom = doc.Nom, Libelle = doc.Libelle, FilePath = $"/api/documents/dossier/{doc.Id}/download" }).ToList()
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