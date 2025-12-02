using BackOffice.Application.Common.DTOs;
using BackOffice.Application.Common.DTOs.Documents;
using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Dossiers.Queries.GetDossierDetail;

/// <summary>
/// Gère la logique de la requête pour récupérer les détails complets d'un dossier.
/// </summary>
public class GetDossierDetailQueryHandler : IRequestHandler<GetDossierDetailQuery, DossierDetailVm>
{
    // Déclaration unique du contexte
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initialise une nouvelle instance du handler.
    /// </summary>
    public GetDossierDetailQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Exécute la requête pour récupérer les détails du dossier.
    /// </summary>
    public async Task<DossierDetailVm> Handle(GetDossierDetailQuery request, CancellationToken cancellationToken)
    {
        // Construire la requête pour charger le dossier et TOUTES ses relations imbriquées.
        // On utilise AsNoTracking() pour une meilleure performance en lecture seule.
        var dossier = await _context.Dossiers
            .AsNoTracking()
            .Where(d => d.Id == request.DossierId)

            // Relations directes du Dossier
            .Include(d => d.Client)
            .Include(d => d.Statut)
            .Include(d => d.ModeReglement)
            .Include(d => d.Commentaires)
            .Include(d => d.Devis)
            .Include(d => d.DocumentsDossiers)

            // Relations liées aux Demandes (Équipements)
            .Include(d => d.Demandes)
                .ThenInclude(dem => dem.Attestations) // Attestations liées à la demande
            .Include(d => d.Demandes)
                .ThenInclude(dem => dem.CategorieEquipement) // Catégorie liée à la demande
            .Include(d => d.Demandes)
                .ThenInclude(dem => dem.MotifRejet) // Motif de rejet lié à la demande
            .Include(d => d.Demandes)
                .ThenInclude(dem => dem.Proposition) // Proposition liée à la demande

            .FirstOrDefaultAsync(cancellationToken);

        // Vérifie si le dossier a été trouvé.
        if (dossier == null)
        {
            throw new Exception($"Le dossier avec l'ID '{request.DossierId}' est introuvable.");
        }

        // Mappe l'entité Dossier et ses relations vers le ViewModel de détail.
        var dossierVm = new DossierDetailVm
        {
            Id = dossier.Id,
            DateOuverture = dossier.DateOuverture,
            Numero = dossier.Numero,
            Libelle = dossier.Libelle,

            Client = dossier.Client != null ? new ClientDto
            {
                Id = dossier.Client.Id,
                RaisonSociale = dossier.Client.RaisonSociale
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

            // Mapping complet des Demandes (Équipements)
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

                // Mapping des objets imbriqués dans la demande
                CategorieEquipement = dem.CategorieEquipement != null ? new CategorieEquipementDto
                {
                    Id = dem.CategorieEquipement.Id,
                    Code = dem.CategorieEquipement.Code,
                    Libelle = dem.CategorieEquipement.Libelle,
                    TypeEquipement = dem.CategorieEquipement.TypeEquipement,
                    TypeClient = dem.CategorieEquipement.TypeClient,
                    TarifEtude = dem.CategorieEquipement.TarifEtude,
                    TarifHomologation = dem.CategorieEquipement.TarifHomologation,
                    TarifControle = dem.CategorieEquipement.TarifControle,
                    FormuleHomologation = dem.CategorieEquipement.FormuleHomologation,
                    QuantiteReference = dem.CategorieEquipement.QuantiteReference,
                    Remarques = dem.CategorieEquipement.Remarques
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

        return dossierVm;
    }
}