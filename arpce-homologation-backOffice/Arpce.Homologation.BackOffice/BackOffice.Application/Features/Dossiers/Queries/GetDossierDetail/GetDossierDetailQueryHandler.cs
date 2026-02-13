using BackOffice.Application.Common.DTOs;
using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

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
            .Include(d => d.Demandes).ThenInclude(dem => dem.Statut)
            .Include(d => d.Demandes).ThenInclude(dem => dem.DocumentsDemandes)
            .Include(d => d.Demandes).ThenInclude(dem => dem.CategorieEquipement)
            .Include(d => d.Demandes).ThenInclude(dem => dem.MotifRejet)
            .Include(d => d.Demandes).ThenInclude(dem => dem.Proposition)
            .Include(d => d.Demandes).ThenInclude(dem => dem.Attestations) 
            .FirstOrDefaultAsync(d => d.Id == request.DossierId, cancellationToken);

        if (dossier == null)
        {
            throw new Exception($"Le dossier avec l'ID '{request.DossierId}' est introuvable.");
        }

        return new DossierDetailVm
        {
            Id = dossier.Id,
            DateOuverture = dossier.DateOuverture,
            Numero = dossier.Numero,
            Libelle = dossier.Libelle,

            Client = dossier.Client != null ? new ClientDto { Id = dossier.Client.Id, RaisonSociale = dossier.Client.RaisonSociale } : null,
            Statut = dossier.Statut != null ? new StatutDto { Id = dossier.Statut.Id, Code = dossier.Statut.Code, Libelle = dossier.Statut.Libelle } : null,
            ModeReglement = dossier.ModeReglement != null ? new ModeReglementDto { Id = dossier.ModeReglement.Id, Code = dossier.ModeReglement.Code, Libelle = dossier.ModeReglement.Libelle } : null,

            Demandes = dossier.Demandes.Select(dem => new DemandeDto
            {
                Id = dem.Id,
                IdDossier = dossier.Id,
                NumeroDemande = dem.NumeroDemande,
                Equipement = dem.Equipement,
                Modele = dem.Modele,
                Marque = dem.Marque,
                Fabricant = dem.Fabricant,
                Type = dem.Type,
                Description = dem.Description,
                QuantiteEquipements = dem.QuantiteEquipements,
                ContactNom = dem.ContactNom,
                ContactEmail = dem.ContactEmail,
                PrixUnitaire = dem.PrixUnitaire,
                Remise = dem.Remise,
                EstHomologable = dem.EstHomologable,
                Statut = dem.Statut != null ? new StatutDto { Id = dem.Statut.Id, Code = dem.Statut.Code, Libelle = dem.Statut.Libelle } : null,
                CategorieEquipement = dem.CategorieEquipement != null ? new CategorieEquipementDto { Id = dem.CategorieEquipement.Id, Code = dem.CategorieEquipement.Code, Libelle = dem.CategorieEquipement.Libelle, FraisEtude = dem.CategorieEquipement.FraisEtude, FraisHomologation = dem.CategorieEquipement.FraisHomologation, FraisControle = dem.CategorieEquipement.FraisControle } : null,
                MotifRejet = dem.MotifRejet != null ? new MotifRejetDto { Id = dem.MotifRejet.Id, Code = dem.MotifRejet.Code, Libelle = dem.MotifRejet.Libelle } : null,
                Proposition = dem.Proposition != null ? new PropositionDto { Id = dem.Proposition.Id, Code = dem.Proposition.Code, Libelle = dem.Proposition.Libelle } : null,
                Documents = dem.DocumentsDemandes.Select(doc => new DocumentDossierDto { Id = doc.Id, Nom = doc.Nom, Extension = doc.Extension, FilePath = $"/api/demandes/demande/{doc.Id}/download" }).ToList()
            }).ToList(),

            Commentaires = dossier.Commentaires.Select(com => new CommentaireDto { Id = com.Id, DateCommentaire = com.DateCommentaire, CommentaireTexte = com.CommentaireTexte, NomInstructeur = com.NomInstructeur }).ToList(),
            Documents = dossier.DocumentsDossiers.Select(doc => new DocumentDossierDto { Id = doc.Id, Nom = doc.Nom, Type = doc.Type, Extension = doc.Extension, FilePath = $"/api/demandes/dossier/{doc.Id}/download" }).ToList(),

            Attestations = dossier.Demandes.SelectMany(dem => dem.Attestations).Select(att => new AttestationDto
            {
                Id = att.Id,
                DateDelivrance = att.DateDelivrance,
                DateExpiration = att.DateExpiration,
                FilePath = $"/api/documents/attestation/{att.Id}/download"
            }).ToList()
        };
    }
}