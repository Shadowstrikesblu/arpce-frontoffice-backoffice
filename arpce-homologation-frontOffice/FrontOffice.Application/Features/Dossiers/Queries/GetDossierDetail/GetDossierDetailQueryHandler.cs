using FrontOffice.Application.Common.DTOs;
using FrontOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace FrontOffice.Application.Features.Dossiers.Queries.GetDossierDetail;

public class GetDossierDetailQueryHandler : IRequestHandler<GetDossierDetailQuery, DossierDetailVm>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetDossierDetailQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _currentUserService = currentUserService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<DossierDetailVm> Handle(GetDossierDetailQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue) throw new UnauthorizedAccessException("Utilisateur non authentifié.");

        var dossier = await _context.Dossiers
            .AsNoTracking()
            .Where(d => d.Id == request.DossierId && d.IdClient == userId.Value)
            .Include(d => d.Statut)
            .Include(d => d.ModeReglement)
            .Include(d => d.Commentaires)
            .Include(d => d.Devis)
            .Include(d => d.DocumentsDossiers)
            .Include(d => d.Demandes).ThenInclude(dem => dem.Statut)
            .Include(d => d.Demandes).ThenInclude(dem => dem.DocumentsDemandes)
            .Include(d => d.Demandes).ThenInclude(dem => dem.Attestations)
            .Include(d => d.Demandes).ThenInclude(dem => dem.CategorieEquipement)
            .Include(d => d.Demandes).ThenInclude(dem => dem.MotifRejet)
            .Include(d => d.Demandes).ThenInclude(dem => dem.Proposition)
            .FirstOrDefaultAsync(cancellationToken);

        if (dossier == null) throw new Exception("Dossier introuvable.");

        return new DossierDetailVm
        {
            Id = dossier.Id,
            DateOuverture = dossier.DateOuverture,
            Numero = dossier.Numero,
            Libelle = dossier.Libelle,

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

                Statut = dem.Statut != null ? new StatutDto
                {
                    Id = dem.Statut.Id,
                    Code = dem.Statut.Code,
                    Libelle = dem.Statut.Libelle
                } : null,

                CategorieEquipement = dem.CategorieEquipement != null ? new CategorieEquipementDto
                {
                    Id = dem.CategorieEquipement.Id,
                    Code = dem.CategorieEquipement.Code,
                    Libelle = dem.CategorieEquipement.Libelle
                } : null,

                MotifRejet = dem.MotifRejet != null ? new MotifRejetDto
                {
                    Id = dem.MotifRejet.Id,
                    Code = dem.MotifRejet.Code,
                    Libelle = dem.MotifRejet.Libelle
                } : null,

                Documents = dem.DocumentsDemandes.Select(doc => new DocumentDossierDto
                {
                    Id = doc.Id,
                    Nom = doc.Nom,
                    Extension = doc.Extension,
                    FilePath = $"/api/documents/demande/{doc.Id}/download"
                }).ToList()
            }).ToList(),

            Devis = dossier.Devis.Select(dev => new DevisDto { Id = dev.Id, MontantEtude = dev.MontantEtude, PaiementOk = dev.PaiementOk }).ToList(),
            Commentaires = dossier.Commentaires.Select(com => new CommentaireDto { Id = com.Id, CommentaireTexte = com.CommentaireTexte, DateCommentaire = com.DateCommentaire }).ToList(),
            Documents = dossier.DocumentsDossiers.Select(doc => new DocumentDossierDto { Id = doc.Id, Nom = doc.Nom, Extension = doc.Extension, FilePath = $"/api/documents/dossier/{doc.Id}/download" }).ToList(),

            Attestations = dossier.Demandes.SelectMany(dem => dem.Attestations).Select(att => new AttestationDto { Id = att.Id, DateDelivrance = att.DateDelivrance, FilePath = $"/api/documents/certificat/{att.Id}/download" }).ToList()
        };
    }
}