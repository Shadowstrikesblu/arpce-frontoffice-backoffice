using BackOffice.Application.Common.DTOs;
using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Dossiers.Queries.GetDossierDetail;

public class GetDossierDetailQueryHandler : IRequestHandler<GetDossierDetailQuery, DossierDetailVm>
{
    private readonly IApplicationDbContext _context;

    public GetDossierDetailQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DossierDetailVm> Handle(GetDossierDetailQuery request, CancellationToken cancellationToken)
    {
        var dossier = await _context.Dossiers
            .AsNoTracking()
            .Include(d => d.Client)
            .Include(d => d.Statut)
            .Include(d => d.ModeReglement)
            .Include(d => d.Commentaires)
            .Include(d => d.Devis)
            .Include(d => d.DocumentsDossiers)
            .Include(d => d.Demande).ThenInclude(dem => dem.Statut)
            .Include(d => d.Demande).ThenInclude(dem => dem.DocumentsDemandes)
            .Include(d => d.Demande).ThenInclude(dem => dem.CategorieEquipement)
            .Include(d => d.Demande).ThenInclude(dem => dem.MotifRejet)
            .Include(d => d.Demande).ThenInclude(dem => dem.Proposition)
            .Include(d => d.Demande).ThenInclude(dem => dem.Attestations)
            .Include(d => d.Demande).ThenInclude(dem => dem.Beneficiaire)
            .FirstOrDefaultAsync(d => d.Id == request.DossierId, cancellationToken);

        if (dossier == null) throw new Exception($"Le dossier avec l'ID '{request.DossierId}' est introuvable.");

        return new DossierDetailVm
        {
            Id = dossier.Id,
            DateOuverture = dossier.DateOuverture,
            Numero = dossier.Numero,
            Libelle = dossier.Libelle,

            // Mapping complet du Client (Redevable) pour éviter les NULLs
            Client = dossier.Client != null ? new ClientDto
            {
                Id = dossier.Client.Id,
                RaisonSociale = dossier.Client.RaisonSociale,
                Adresse = dossier.Client.Adresse,
                Ville = dossier.Client.Ville,
                Pays = dossier.Client.Pays,
                ContactNom = dossier.Client.ContactNom,
                ContactTelephone = dossier.Client.ContactTelephone,
                Email = dossier.Client.Email,
                Bp = dossier.Client.Bp
            } : null,

            Statut = dossier.Statut != null ? new StatutDto
            {
                Id = dossier.Statut.Id,
                Code = dossier.Statut.Code,
                Libelle = dossier.Statut.Libelle
            } : null,

            ModeReglement = dossier.ModeReglement != null ? new ModeReglementDto
            {
                Id = dossier.ModeReglement.Id,
                Code = dossier.ModeReglement.Code,
                Libelle = dossier.ModeReglement.Libelle
            } : null,

            // Mapping de l'unique demande (1:1)
            Demande = dossier.Demande != null ? new DemandeDto
            {
                Id = dossier.Demande.Id,
                IdDossier = dossier.Id, // On force l'ID du dossier parent
                NumeroDemande = dossier.Demande.NumeroDemande,
                Equipement = dossier.Demande.Equipement,
                Modele = dossier.Demande.Modele,
                Marque = dossier.Demande.Marque,
                Fabricant = dossier.Demande.Fabricant,
                Type = dossier.Demande.Type,
                Description = dossier.Demande.Description,
                QuantiteEquipements = dossier.Demande.QuantiteEquipements ?? 0,
                PrixUnitaire = dossier.Demande.PrixUnitaire,
                EstHomologable = dossier.Demande.EstHomologable,
                RequiertEchantillon = dossier.Demande.RequiertEchantillon,
                EchantillonSoumis = dossier.Demande.EchantillonSoumis,

                // Mapping du bénéficiaire lié à la demande (avec ID corrigé)
                Beneficiaire = dossier.Demande.Beneficiaire != null ? new BeneficiaireDto
                {
                    Id = dossier.Demande.Beneficiaire.Id,
                    Nom = dossier.Demande.Beneficiaire.Nom,
                    Email = dossier.Demande.Beneficiaire.Email,
                    Telephone = dossier.Demande.Beneficiaire.Telephone,
                    Adresse = dossier.Demande.Beneficiaire.Adresse,
                    Type = dossier.Demande.Beneficiaire.Type
                } : null,

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
                    Libelle = dossier.Demande.CategorieEquipement.Libelle,
                    FraisEtude = dossier.Demande.CategorieEquipement.FraisEtude,
                    FraisHomologation = dossier.Demande.CategorieEquipement.FraisHomologation,
                    FraisControle = dossier.Demande.CategorieEquipement.FraisControle,
                    ModeCalcul = dossier.Demande.CategorieEquipement.ModeCalcul,
                    BlockSize = dossier.Demande.CategorieEquipement.BlockSize,
                    ReferenceLoiFinance = dossier.Demande.CategorieEquipement.ReferenceLoiFinance
                } : null,

                MotifRejet = dossier.Demande.MotifRejet != null ? new MotifRejetDto
                {
                    Id = dossier.Demande.MotifRejet.Id,
                    Code = dossier.Demande.MotifRejet.Code,
                    Libelle = dossier.Demande.MotifRejet.Libelle
                } : null,

                Documents = dossier.Demande.DocumentsDemandes.Select(doc => new DocumentDossierDto
                {
                    Id = doc.Id,
                    Nom = doc.Nom,
                    Libelle = doc.Libelle,
                    Extension = doc.Extension,
                    FilePath = $"/api/demandes/demande/{doc.Id}/download"
                }).ToList()
            } : null,

            Commentaires = dossier.Commentaires.Select(com => new CommentaireDto
            {
                Id = com.Id,
                CommentaireTexte = com.CommentaireTexte,
                NomInstructeur = com.NomInstructeur,
                DateCommentaire = com.DateCommentaire
            }).ToList(),

            Documents = dossier.DocumentsDossiers.Select(doc => new DocumentDossierDto
            {
                Id = doc.Id,
                Nom = doc.Nom,
                Libelle = doc.Libelle,
                Type = doc.Type,
                Extension = doc.Extension,
                FilePath = $"/api/demandes/dossier/{doc.Id}/download"
            }).ToList(),

            // Règle : Devis uniquement si homologable
            Devis = (dossier.Demande != null && dossier.Demande.EstHomologable)
                ? dossier.Devis.Select(dev => new DevisDto
                {
                    Id = dev.Id,
                    Date = dev.Date,
                    MontantEtude = dev.MontantEtude,
                    MontantHomologation = dev.MontantHomologation,
                    MontantControle = dev.MontantControle,
                    MontantPenalite = dev.MontantPenalite,
                    MontantTotal = dev.MontantEtude + (dev.MontantHomologation ?? 0) + (dev.MontantControle ?? 0) + dev.MontantPenalite,
                    PaiementOk = dev.PaiementOk,
                    FilePath = $"/api/devis/{dev.Id}/download"
                }).ToList()
                : new List<DevisDto>(),

            Attestations = dossier.Demande?.Attestations.Select(att => new AttestationDto
            {
                Id = att.Id,
                DateDelivrance = att.DateDelivrance,
                DateExpiration = att.DateExpiration,
                FilePath = $"/api/documents/attestation/{att.Id}/download"
            }).ToList() ?? new List<AttestationDto>()
        };
    }
}