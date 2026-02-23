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
            .Include(d => d.Demande).ThenInclude(dem => dem.Statut)
            .Include(d => d.Demande).ThenInclude(dem => dem.DocumentsDemandes) 
            .Include(d => d.Demande).ThenInclude(dem => dem.Attestations)
            .Include(d => d.Demande).ThenInclude(dem => dem.CategorieEquipement)
            .Include(d => d.Demande).ThenInclude(dem => dem.MotifRejet)
            .Include(d => d.Demande).ThenInclude(dem => dem.Proposition)
            .Include(d => d.Demande).ThenInclude(dem => dem.Beneficiaire) 
            .FirstOrDefaultAsync(cancellationToken);

        if (dossier == null) throw new Exception("Dossier introuvable.");

        return new DossierDetailVm
        {
            Id = dossier.Id,
            DateOuverture = dossier.DateOuverture,
            Numero = dossier.Numero,
            Libelle = dossier.Libelle,

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
                    RequiertEchantillon = dossier.Demande.RequiertEchantillon,
                    EchantillonSoumis = dossier.Demande.EchantillonSoumis,

                    Statut = dossier.Demande.Statut != null ? new StatutDto { Id = dossier.Demande.Statut.Id, Code = dossier.Demande.Statut.Code, Libelle = dossier.Demande.Statut.Libelle } : null,
                    CategorieEquipement = dossier.Demande.CategorieEquipement != null ? new CategorieEquipementDto { Id = dossier.Demande.CategorieEquipement.Id, Code = dossier.Demande.CategorieEquipement.Code, Libelle = dossier.Demande.CategorieEquipement.Libelle } : null,
                    MotifRejet = dossier.Demande.MotifRejet != null ? new MotifRejetDto { Id = dossier.Demande.MotifRejet.Id, Code = dossier.Demande.MotifRejet.Code, Libelle = dossier.Demande.MotifRejet.Libelle } : null,

                    Beneficiaire = dossier.Demande.Beneficiaire != null ? new BeneficiaireDto
                    {
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

            Devis = dossier.Devis.Select(dev => new DevisDto { Id = dev.Id, MontantEtude = dev.MontantEtude, PaiementOk = dev.PaiementOk }).ToList(),
            Commentaires = dossier.Commentaires.Select(com => new CommentaireDto { Id = com.Id, CommentaireTexte = com.CommentaireTexte, DateCommentaire = com.DateCommentaire }).ToList(),

            Documents = dossier.DocumentsDossiers.Select(doc => new DocumentDossierDto
            {
                Id = doc.Id,
                Nom = doc.Nom,
                Extension = doc.Extension,
                FilePath = $"/api/documents/dossier/{doc.Id}/download"
            }).ToList(),

            Attestations = dossier.Demande != null
                ? dossier.Demande.Attestations
                    .Where(att => att.Donnees != null && att.Donnees.Length > 0)
                    .Select(att => new AttestationDto { Id = att.Id, DateDelivrance = att.DateDelivrance, FilePath = $"/api/documents/certificat/{att.Id}/download" }).ToList()
                : new List<AttestationDto>()
        };
    }
}