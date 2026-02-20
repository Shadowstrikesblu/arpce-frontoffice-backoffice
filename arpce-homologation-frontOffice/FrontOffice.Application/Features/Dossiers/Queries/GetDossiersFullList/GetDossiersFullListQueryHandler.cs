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
            .Include(d => d.ModeReglement)
            .Include(d => d.Commentaires)
            .Include(d => d.Devis)
            .Include(d => d.DocumentsDossiers)
            .Include(d => d.Demande).ThenInclude(dem => dem.Statut) 
            .Include(d => d.Demande).ThenInclude(dem => dem.Attestations)
            .Include(d => d.Demande).ThenInclude(dem => dem.CategorieEquipement)
            .Include(d => d.Demande).ThenInclude(dem => dem.MotifRejet)
            .Include(d => d.Demande).ThenInclude(dem => dem.Proposition)
            .Include(d => d.Demande).ThenInclude(dem => dem.DocumentsDemandes)
            .ToListAsync(cancellationToken);

        var dtos = dossiers.Select(d => new DossierFullListItemDto
        {
            Id = d.Id,
            Numero = d.Numero,
            Libelle = d.Libelle,
            DateOuverture = d.DateOuverture,
            NbDemandes = d.Demande != null ? 1 : 0, 

            Statut = d.Statut != null ? new StatutDto { Id = d.Statut.Id, Code = d.Statut.Code, Libelle = d.Statut.Libelle } : null,
            ModeReglement = d.ModeReglement != null ? new ModeReglementDto { Id = d.ModeReglement.Id, Code = d.ModeReglement.Code, Libelle = d.ModeReglement.Libelle } : null,

            Commentaires = d.Commentaires.Select(com => new CommentaireDto { Id = com.Id, DateCommentaire = com.DateCommentaire, CommentaireTexte = com.CommentaireTexte, NomInstructeur = com.NomInstructeur }).ToList(),
            Devis = d.Devis.Select(dev => new DevisDto { Id = dev.Id, Date = dev.Date, MontantEtude = dev.MontantEtude, MontantHomologation = dev.MontantHomologation, PaiementOk = dev.PaiementOk }).ToList(),
            Documents = d.DocumentsDossiers.Select(doc => new DocumentDossierDto { Id = doc.Id, Nom = doc.Nom, Extension = doc.Extension, FilePath = $"/api/documents/dossier/{doc.Id}/download" }).ToList(),

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
                    QuantiteEquipements = d.Demande.QuantiteEquipements,
                    PrixUnitaire = d.Demande.PrixUnitaire,
                    Remise = d.Demande.Remise,
                    EstHomologable = d.Demande.EstHomologable,

                    Statut = d.Demande.Statut != null ? new StatutDto
                    {
                        Id = d.Demande.Statut.Id,
                        Code = d.Demande.Statut.Code,
                        Libelle = d.Demande.Statut.Libelle
                    } : null,

                    CategorieEquipement = d.Demande.CategorieEquipement != null ? new CategorieEquipementDto { Id = d.Demande.CategorieEquipement.Id, Code = d.Demande.CategorieEquipement.Code, Libelle = d.Demande.CategorieEquipement.Libelle } : null,
                    MotifRejet = d.Demande.MotifRejet != null ? new MotifRejetDto { Id = d.Demande.MotifRejet.Id, Code = d.Demande.MotifRejet.Code, Libelle = d.Demande.MotifRejet.Libelle } : null,
                    Proposition = d.Demande.Proposition != null ? new PropositionDto { Id = d.Demande.Proposition.Id, Code = d.Demande.Proposition.Code, Libelle = d.Demande.Proposition.Libelle } : null,
                    Documents = d.Demande.DocumentsDemandes.Select(doc => new DocumentDossierDto { Id = doc.Id, Nom = doc.Nom, Extension = doc.Extension, FilePath = $"/api/documents/demande/{doc.Id}/download" }).ToList()
                }
            } : new List<DemandeDto>(),

            Attestations = d.Demande != null ? d.Demande.Attestations.Select(att => new AttestationDto { Id = att.Id, DateDelivrance = att.DateDelivrance, DateExpiration = att.DateExpiration, FilePath = $"/api/documents/certificat/{att.Id}/download" }).ToList() : new List<AttestationDto>()
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