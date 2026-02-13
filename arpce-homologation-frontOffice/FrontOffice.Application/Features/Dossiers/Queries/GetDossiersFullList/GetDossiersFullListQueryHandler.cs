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

        // Filtres recherche
        if (!string.IsNullOrWhiteSpace(request.Parameters.Recherche))
        {
            var term = request.Parameters.Recherche.Trim().ToLower();
            query = query.Where(d => d.Numero.ToLower().Contains(term) || d.Libelle.ToLower().Contains(term));
        }

        // Tri dynamique
        if (!string.IsNullOrWhiteSpace(request.Parameters.TrierPar))
        {
            bool isDescending = request.Parameters.Ordre?.ToLower() == "desc";
            query = request.Parameters.TrierPar.ToLower() switch
            {
                "date_creation" or "dateouverture" => isDescending ? query.OrderByDescending(d => d.DateOuverture) : query.OrderBy(d => d.DateOuverture),
                "libelle" => isDescending ? query.OrderByDescending(d => d.Libelle) : query.OrderBy(d => d.Libelle),
                "statut" => isDescending ? query.OrderByDescending(d => d.Statut.Libelle) : query.OrderBy(d => d.Statut.Libelle),
                _ => query.OrderByDescending(d => d.DateOuverture)
            };
        }
        else
        {
            query = query.OrderByDescending(d => d.DateOuverture);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        // Chargement complet avec inclusions profondes
        var dossiers = await query
            .Skip((request.Parameters.Page - 1) * request.Parameters.TaillePage)
            .Take(request.Parameters.TaillePage)
            .Include(d => d.Statut)
            .Include(d => d.ModeReglement)
            .Include(d => d.Commentaires)
            .Include(d => d.Devis)
            .Include(d => d.DocumentsDossiers)
            .Include(d => d.Demandes).ThenInclude(dem => dem.Statut) 
            .Include(d => d.Demandes).ThenInclude(dem => dem.Attestations)
            .Include(d => d.Demandes).ThenInclude(dem => dem.CategorieEquipement)
            .Include(d => d.Demandes).ThenInclude(dem => dem.MotifRejet)
            .Include(d => d.Demandes).ThenInclude(dem => dem.Proposition)
            .Include(d => d.Demandes).ThenInclude(dem => dem.DocumentsDemandes)
            .ToListAsync(cancellationToken);

        var dtos = dossiers.Select(d => new DossierFullListItemDto
        {
            Id = d.Id,
            Numero = d.Numero,
            Libelle = d.Libelle,
            DateOuverture = d.DateOuverture,
            NbDemandes = d.Demandes.Count,

            Statut = d.Statut != null ? new StatutDto { Id = d.Statut.Id, Code = d.Statut.Code, Libelle = d.Statut.Libelle } : null,
            ModeReglement = d.ModeReglement != null ? new ModeReglementDto { Id = d.ModeReglement.Id, Code = d.ModeReglement.Code, Libelle = d.ModeReglement.Libelle } : null,

            Commentaires = d.Commentaires.Select(com => new CommentaireDto { Id = com.Id, DateCommentaire = com.DateCommentaire, CommentaireTexte = com.CommentaireTexte, NomInstructeur = com.NomInstructeur }).ToList(),
            Devis = d.Devis.Select(dev => new DevisDto { Id = dev.Id, Date = dev.Date, MontantEtude = dev.MontantEtude, MontantHomologation = dev.MontantHomologation, PaiementOk = dev.PaiementOk }).ToList(),
            Documents = d.DocumentsDossiers.Select(doc => new DocumentDossierDto { Id = doc.Id, Nom = doc.Nom, Extension = doc.Extension, FilePath = $"/api/documents/dossier/{doc.Id}/download" }).ToList(),

            Demandes = d.Demandes.Select(dem => new DemandeDto
            {
                Id = dem.Id,
                IdDossier = d.Id,
                Equipement = dem.Equipement,
                Modele = dem.Modele,
                Marque = dem.Marque,
                Fabricant = dem.Fabricant,
                Type = dem.Type,
                QuantiteEquipements = dem.QuantiteEquipements,
                PrixUnitaire = dem.PrixUnitaire,
                Remise = dem.Remise,
                EstHomologable = dem.EstHomologable,

                Statut = dem.Statut != null ? new StatutDto
                {
                    Id = dem.Statut.Id,
                    Code = dem.Statut.Code,
                    Libelle = dem.Statut.Libelle
                } : null,

                CategorieEquipement = dem.CategorieEquipement != null ? new CategorieEquipementDto { Id = dem.CategorieEquipement.Id, Code = dem.CategorieEquipement.Code, Libelle = dem.CategorieEquipement.Libelle } : null,
                MotifRejet = dem.MotifRejet != null ? new MotifRejetDto { Id = dem.MotifRejet.Id, Code = dem.MotifRejet.Code, Libelle = dem.MotifRejet.Libelle } : null,
                Proposition = dem.Proposition != null ? new PropositionDto { Id = dem.Proposition.Id, Code = dem.Proposition.Code, Libelle = dem.Proposition.Libelle } : null,
                Documents = dem.DocumentsDemandes.Select(doc => new DocumentDossierDto { Id = doc.Id, Nom = doc.Nom, Extension = doc.Extension, FilePath = $"/api/documents/demande/{doc.Id}/download" }).ToList()
            }).ToList(),

            Attestations = d.Demandes.SelectMany(dem => dem.Attestations).Select(att => new AttestationDto { Id = att.Id, DateDelivrance = att.DateDelivrance, DateExpiration = att.DateExpiration, FilePath = $"/api/documents/certificat/{att.Id}/download" }).ToList()
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