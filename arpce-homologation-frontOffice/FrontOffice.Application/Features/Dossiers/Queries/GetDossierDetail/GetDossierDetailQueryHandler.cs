using FrontOffice.Application.Common.DTOs;
using FrontOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FrontOffice.Application.Features.Dossiers.Queries.GetDossierDetail;

/// <summary>
/// Gère la logique de la requête pour récupérer les détails complets d'un dossier.
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
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("Utilisateur non authentifié.");
        }

        // Construction de la requête pour récupérer le dossier et toutes ses relations.
        var dossier = await _context.Dossiers
            .AsNoTracking()
            .Where(d => d.Id == request.DossierId && d.IdClient == userId.Value)
            .Include(d => d.Statut)
            .Include(d => d.ModeReglement)
            .Include(d => d.Commentaires)
            .Include(d => d.Devis)
            .Include(d => d.DocumentsDossiers)
            .Include(d => d.Demandes)
                .ThenInclude(dem => dem.Attestations) 
            .FirstOrDefaultAsync(cancellationToken);

        // Vérifie si le dossier a été trouvé
        if (dossier == null)
        {
            throw new Exception($"Le dossier avec l'ID '{request.DossierId}' est introuvable ou vous n'y avez pas accès.");
        }

        // Mappe l'entité Dossier et ses relations vers le ViewModel de détail.
        var dossierVm = new DossierDetailVm
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
                Marque = dem.Marque
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

            Attestations = dossier.Demandes
                .SelectMany(dem => dem.Attestations) 
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