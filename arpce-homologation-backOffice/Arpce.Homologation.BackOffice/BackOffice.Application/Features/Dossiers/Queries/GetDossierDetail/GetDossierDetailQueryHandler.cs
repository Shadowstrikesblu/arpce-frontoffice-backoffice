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
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetDossierDetailQueryHandler(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<DossierDetailVm> Handle(GetDossierDetailQuery request, CancellationToken cancellationToken)
    {
        var dossier = await _context.Dossiers
            .AsNoTracking()
            .Include(d => d.Client)
            .Include(d => d.Statut)
            .Include(d => d.ModeReglement)
            .Include(d => d.Commentaires)
            .Include(d => d.DocumentsDossiers)
            .Include(d => d.Devis)
            .Include(d => d.Demande).ThenInclude(dem => dem.Statut)
            .Include(d => d.Demande).ThenInclude(dem => dem.DocumentsDemandes)
            .Include(d => d.Demande).ThenInclude(dem => dem.CategorieEquipement)
            .Include(d => d.Demande).ThenInclude(dem => dem.MotifRejet)
            .Include(d => d.Demande).ThenInclude(dem => dem.Proposition)
            .Include(d => d.Demande).ThenInclude(dem => dem.Attestations)
            .FirstOrDefaultAsync(d => d.Id == request.DossierId, cancellationToken);

        if (dossier == null) throw new Exception($"Le dossier avec l'ID '{request.DossierId}' est introuvable.");

        return new DossierDetailVm
        {
            Id = dossier.Id,
            DateOuverture = dossier.DateOuverture,
            Numero = dossier.Numero,
            Libelle = dossier.Libelle,
            Client = dossier.Client != null ? new ClientDto { Id = dossier.Client.Id, RaisonSociale = dossier.Client.RaisonSociale } : null,
            Statut = dossier.Statut != null ? new StatutDto { Id = dossier.Statut.Id, Code = dossier.Statut.Code, Libelle = dossier.Statut.Libelle } : null,
            ModeReglement = dossier.ModeReglement != null ? new ModeReglementDto { Id = dossier.ModeReglement.Id, Code = dossier.ModeReglement.Code, Libelle = dossier.ModeReglement.Libelle } : null,

            Demandes = dossier.Demande != null ? new List<DemandeDto>
            {
                new DemandeDto
                {
                    Id = dossier.Demande.Id,
                    IdDossier = dossier.Id,
                    NumeroDemande = dossier.Demande.NumeroDemande,
                    Equipement = dossier.Demande.Equipement,
                    Modele = dossier.Demande.Modele,
                    Marque = dossier.Demande.Marque,
                    Fabricant = dossier.Demande.Fabricant,
                    Type = dossier.Demande.Type,
                    Description = dossier.Demande.Description,
                    QuantiteEquipements = dossier.Demande.QuantiteEquipements,
                    ContactNom = dossier.Demande.ContactNom,
                    ContactEmail = dossier.Demande.ContactEmail,
                    PrixUnitaire = dossier.Demande.PrixUnitaire,
                    Remise = dossier.Demande.Remise,
                    EstHomologable = dossier.Demande.EstHomologable,
                    Statut = dossier.Demande.Statut != null ? new StatutDto { Id = dossier.Demande.Statut.Id, Code = dossier.Demande.Statut.Code, Libelle = dossier.Demande.Statut.Libelle } : null,
                    CategorieEquipement = dossier.Demande.CategorieEquipement != null ? new CategorieEquipementDto { Id = dossier.Demande.CategorieEquipement.Id, Code = dossier.Demande.CategorieEquipement.Code, Libelle = dossier.Demande.CategorieEquipement.Libelle, FraisEtude = dossier.Demande.CategorieEquipement.FraisEtude, FraisHomologation = dossier.Demande.CategorieEquipement.FraisHomologation, FraisControle = dossier.Demande.CategorieEquipement.FraisControle } : null,
                    MotifRejet = dossier.Demande.MotifRejet != null ? new MotifRejetDto { Id = dossier.Demande.MotifRejet.Id, Code = dossier.Demande.MotifRejet.Code, Libelle = dossier.Demande.MotifRejet.Libelle } : null,
                    Proposition = dossier.Demande.Proposition != null ? new PropositionDto { Id = dossier.Demande.Proposition.Id, Code = dossier.Demande.Proposition.Code, Libelle = dossier.Demande.Proposition.Libelle } : null,
                    Documents = dossier.Demande.DocumentsDemandes.Select(doc => new DocumentDossierDto { Id = doc.Id, Nom = doc.Nom, Extension = doc.Extension, FilePath = $"/api/demandes/demande/{doc.Id}/download" }).ToList()
                }
            } : new List<DemandeDto>(),

            Commentaires = dossier.Commentaires.Select(com => new CommentaireDto { Id = com.Id, DateCommentaire = com.DateCommentaire, CommentaireTexte = com.CommentaireTexte, NomInstructeur = com.NomInstructeur }).ToList(),
            Documents = dossier.DocumentsDossiers.Select(doc => new DocumentDossierDto { Id = doc.Id, Nom = doc.Nom, Type = doc.Type, Extension = doc.Extension, FilePath = $"/api/demandes/dossier/{doc.Id}/download" }).ToList(),
            Devis = dossier.Devis.Select(dev => new DevisDto { Id = dev.Id, Date = dev.Date, MontantEtude = dev.MontantEtude, MontantHomologation = dev.MontantHomologation, MontantControle = dev.MontantControle, PaiementOk = dev.PaiementOk, FilePath = $"/api/devis/{dev.Id}/download" }).ToList(),

            Attestations = dossier.Demande != null
                ? dossier.Demande.Attestations.Select(att => new AttestationDto { Id = att.Id, DateDelivrance = att.DateDelivrance, DateExpiration = att.DateExpiration, FilePath = $"/api/documents/attestation/{att.Id}/download" }).ToList()
                : new List<AttestationDto>()
        };
    }
}