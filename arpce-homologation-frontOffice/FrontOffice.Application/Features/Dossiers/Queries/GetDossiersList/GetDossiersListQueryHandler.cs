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
            .Include(d => d.DocumentsDossiers)
            .Include(d => d.Devis)
            .Include(d => d.Demande).ThenInclude(dem => dem.Statut)
            .Include(d => d.Demande).ThenInclude(dem => dem.Attestations)
            .Include(d => d.Demande).ThenInclude(dem => dem.DocumentsDemandes)
            .Include(d => d.Demande).ThenInclude(dem => dem.Beneficiaire)
            // --- INCLUSIONS MANQUANTES AJOUTÉES ---
            .Include(d => d.Demande).ThenInclude(dem => dem.CategorieEquipement)
            .Include(d => d.Demande).ThenInclude(dem => dem.MotifRejet)
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

            // MAPPING COMPLET DES DOCUMENTS DU DOSSIER (Avec Libellé et Type)
            Documents = dossier.DocumentsDossiers.Select(doc => new DocumentDossierDto
            {
                Id = doc.Id,
                Nom = doc.Nom,
                Libelle = doc.Libelle,
                Type = doc.Type,
                Extension = doc.Extension,
                FilePath = $"/api/documents/dossier/{doc.Id}/download"
            }).ToList(),

            Devis = dossier.Devis?.Select(dev => new DevisDto
            {
                Id = dev.Id,
                Date = dev.Date,
                MontantEtude = dev.MontantEtude,
                MontantHomologation = dev.MontantHomologation,
                MontantControle = dev.MontantControle,
                PaiementOk = dev.PaiementOk
            }).ToList() ?? new(),

            Demandes = dossier.Demande != null ? new List<DemandeDto>
            {
                new DemandeDto
                {
                    Id = dossier.Demande.Id,
                    IdDossier = dossier.Id,
                    Equipement = dossier.Demande.Equipement,
                    Modele = dossier.Demande.Modele,
                    Marque = dossier.Demande.Marque,
                    Fabricant = dossier.Demande.Fabricant,
                    Type = dossier.Demande.Type,
                    Description = dossier.Demande.Description,
                    QuantiteEquipements = dossier.Demande.QuantiteEquipements,
                    PrixUnitaire = dossier.Demande.PrixUnitaire,
                    Remise = dossier.Demande.Remise,
                    EstHomologable = dossier.Demande.EstHomologable,
                    RequiertEchantillon = dossier.Demande.RequiertEchantillon,
                    EchantillonSoumis = dossier.Demande.EchantillonSoumis,

                    Statut = dossier.Demande.Statut != null ? new StatutDto
                    {
                        Id = dossier.Demande.Statut.Id,
                        Code = dossier.Demande.Statut.Code,
                        Libelle = dossier.Demande.Statut.Libelle
                    } : null,

                    CategorieEquipement = dossier.Demande.CategorieEquipement != null ? new CategorieEquipementDto
                    {
                        Id = dossier.Demande.CategorieEquipement.Id,
                        Code = dossier.Demande.CategorieEquipement.Code,
                        Libelle = dossier.Demande.CategorieEquipement.Libelle
                    } : null,

                    MotifRejet = dossier.Demande.MotifRejet != null ? new MotifRejetDto
                    {
                        Id = dossier.Demande.MotifRejet.Id,
                        Code = dossier.Demande.MotifRejet.Code,
                        Libelle = dossier.Demande.MotifRejet.Libelle
                    } : null,

                    Beneficiaire = dossier.Demande.Beneficiaire != null ? new BeneficiaireDto
                    {
                        //Id = dossier.Demande.Beneficiaire.Id,
                        Nom = dossier.Demande.Beneficiaire.Nom,
                        Email = dossier.Demande.Beneficiaire.Email,
                        Telephone = dossier.Demande.Beneficiaire.Telephone,
                        Type = dossier.Demande.Beneficiaire.Type,
                        Adresse = dossier.Demande.Beneficiaire.Adresse
                    } : null,

                    Documents = dossier.Demande.DocumentsDemandes.Select(doc => new DocumentDossierDto
                    {
                        Id = doc.Id,
                        Nom = doc.Nom,
                        Libelle = doc.Libelle,
                        Extension = doc.Extension,
                        FilePath = $"/api/documents/demande/{doc.Id}/download"
                    }).ToList()
                }
            } : new List<DemandeDto>(),

            Attestations = dossier.Demande != null
                ? dossier.Demande.Attestations
                    .Where(att => att.Donnees != null && att.Donnees.Length > 0)
                    .Select(att => new AttestationDto
                    {
                        Id = att.Id,
                        DateDelivrance = att.DateDelivrance,
                        DateExpiration = att.DateExpiration,
                        FilePath = $"/api/documents/certificat/{att.Id}/download"
                    }).ToList()
                : new List<AttestationDto>()

        }).ToList();

        return new DossiersListVm
        {
            Dossiers = dossierDtos,
            Page = request.Parameters.Page,
            PageTaille = request.Parameters.TaillePage,
            TotalPage = (int)Math.Ceiling(totalCount / (double)request.Parameters.TaillePage),
            Recherche = request.Parameters.Recherche
        };
    }
}