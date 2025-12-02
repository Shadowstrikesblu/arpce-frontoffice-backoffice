using FrontOffice.Application.Common.DTOs;
using FrontOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FrontOffice.Application.Features.Dossiers.Queries.GetDossierDetail;

/// <summary>
/// Gère la logique de la requête pour récupérer les détails complets d'un dossier pour le client connecté.
/// </summary>
public class GetDossierDetailQueryHandler : IRequestHandler<GetDossierDetailQuery, DossierDetailVm>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    /// Initialise une nouvelle instance du handler.
    /// </summary>
    public GetDossierDetailQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Exécute la requête pour récupérer les détails du dossier.
    /// </summary>
    public async Task<DossierDetailVm> Handle(GetDossierDetailQuery request, CancellationToken cancellationToken)
    {
        // 1. Validation de l'utilisateur
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("Utilisateur non authentifié.");
        }

        // 2. Requête pour récupérer le dossier et TOUTES ses relations
        // AsNoTracking est utilisé pour la performance (lecture seule).
        var dossier = await _context.Dossiers
            .AsNoTracking()
            // Filtre de sécurité : le dossier doit appartenir à l'utilisateur connecté.
            .Where(d => d.Id == request.DossierId && d.IdClient == userId.Value)

            // Relations directes du Dossier
            .Include(d => d.Statut)
            .Include(d => d.ModeReglement)
            .Include(d => d.Commentaires)
            .Include(d => d.Devis)
            .Include(d => d.DocumentsDossiers)

            // Relations imbriquées des Demandes (Équipements)
            .Include(d => d.Demandes).ThenInclude(dem => dem.Attestations)
            .Include(d => d.Demandes).ThenInclude(dem => dem.CategorieEquipement)
            .Include(d => d.Demandes).ThenInclude(dem => dem.MotifRejet)
            .Include(d => d.Demandes).ThenInclude(dem => dem.Proposition)

            .FirstOrDefaultAsync(cancellationToken);

        // 3. Vérification de l'existence du dossier
        if (dossier == null)
        {
            throw new Exception($"Le dossier avec l'ID '{request.DossierId}' est introuvable ou vous n'y avez pas accès.");
        }

        // 4. Mapping complet vers le ViewModel
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
                IdDossier = dem.IdDossier,
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

                CategorieEquipement = dem.CategorieEquipement != null ? new CategorieEquipementDto
                {
                    Id = dem.CategorieEquipement.Id,
                    Code = dem.CategorieEquipement.Code,
                    Libelle = dem.CategorieEquipement.Libelle,
                    TarifEtude = dem.CategorieEquipement.TarifEtude,
                    TarifHomologation = dem.CategorieEquipement.TarifHomologation,
                } : null,

                MotifRejet = dem.MotifRejet != null ? new MotifRejetDto
                {
                    Id = dem.MotifRejet.Id,
                    Code = dem.MotifRejet.Code,
                    Libelle = dem.MotifRejet.Libelle,
                    Remarques = dem.MotifRejet.Remarques
                } : null,

                Proposition = dem.Proposition != null ? new PropositionDto
                {
                    Id = dem.Proposition.Id,
                    Code = dem.Proposition.Code,
                    Libelle = dem.Proposition.Libelle
                } : null
            }).ToList(),

            Devis = dossier.Devis.Select(dev => new DevisDto
            {
                Id = dev.Id,
                Date = dev.Date,
                MontantEtude = dev.MontantEtude,
                MontantHomologation = dev.MontantHomologation,
                MontantControle = dev.MontantControle,
                PaiementOk = dev.PaiementOk
            }).ToList(),

            Commentaires = dossier.Commentaires.Select(com => new CommentaireDto
            {
                Id = com.Id,
                DateCommentaire = com.DateCommentaire,
                CommentaireTexte = com.CommentaireTexte,
                NomInstructeur = com.NomInstructeur
            }).ToList(),

            Documents = dossier.DocumentsDossiers.Select(doc => new DocumentDossierDto
            {
                Id = doc.Id,
                Nom = doc.Nom,
                Type = doc.Type,
                Extension = doc.Extension,
                FilePath = doc.FilePath
            }).ToList(),

            Attestations = dossier.Demandes.SelectMany(dem => dem.Attestations)
                .Select(att => new AttestationDto
                {
                    Id = att.Id,
                    DateDelivrance = att.DateDelivrance,
                    DateExpiration = att.DateExpiration
                }).ToList()
        };
    }
}