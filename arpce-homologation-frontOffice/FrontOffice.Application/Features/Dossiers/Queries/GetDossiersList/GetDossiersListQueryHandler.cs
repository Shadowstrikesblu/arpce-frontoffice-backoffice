using FrontOffice.Application.Common.DTOs;
using FrontOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FrontOffice.Application.Features.Dossiers.Queries.GetDossiersList;

public class GetDossiersListQueryHandler : IRequestHandler<GetDossiersListQuery, DossiersListVm>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetDossiersListQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<DossiersListVm> Handle(GetDossiersListQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue) throw new UnauthorizedAccessException("Utilisateur non authentifié.");

        var query = _context.Dossiers
            .AsNoTracking()
            .Where(d => d.IdClient == userId.Value)
            .Include(d => d.Statut)
            .Include(d => d.DocumentsDossiers) 
            .Include(d => d.Devis) 
            .Include(d => d.Demande).ThenInclude(dem => dem.Statut)
            .Include(d => d.Demande).ThenInclude(dem => dem.Attestations)
            .Include(d => d.Demande).ThenInclude(dem => dem.DocumentsDemandes)
            .Include(d => d.Demande).ThenInclude(dem => dem.Beneficiaire) 
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Parameters.Recherche))
        {
            var search = request.Parameters.Recherche.ToLower();
            query = query.Where(d => d.Numero.ToLower().Contains(search) || d.Libelle.ToLower().Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var dossiersPaged = await query
            .OrderByDescending(d => d.DateOuverture)
            .Skip((request.Parameters.Page - 1) * request.Parameters.TaillePage)
            .Take(request.Parameters.TaillePage)
            .ToListAsync(cancellationToken);

        var dossierDtos = dossiersPaged.Select(dossier => new DossierListItemDto
        {
            Id = dossier.Id,
            DateOuverture = dossier.DateOuverture,
            Numero = dossier.Numero,
            Libelle = dossier.Libelle,
            Statut = dossier.Statut != null ? new StatutDto { Id = dossier.Statut.Id, Code = dossier.Statut.Code, Libelle = dossier.Statut.Libelle } : null,

            Documents = dossier.DocumentsDossiers.Select(doc => new DocumentDossierDto
            {
                Id = doc.Id,
                Nom = doc.Nom,
                Extension = doc.Extension,
                FilePath = $"/api/documents/dossier/{doc.Id}/download"
            }).ToList(),

            Devis = dossier.Devis?.Select(dev => new DevisDto { Id = dev.Id, PaiementOk = dev.PaiementOk }).ToList() ?? new(),

            Demandes = dossier.Demande != null ? new List<DemandeDto>
            {
                new DemandeDto
                {
                    Id = dossier.Demande.Id,
                    Equipement = dossier.Demande.Equipement,
                    Modele = dossier.Demande.Modele,
                    Marque = dossier.Demande.Marque,
                    Fabricant = dossier.Demande.Fabricant,
                    Type = dossier.Demande.Type,
                    Description = dossier.Demande.Description,
                    QuantiteEquipements = dossier.Demande.QuantiteEquipements,
                    PrixUnitaire = dossier.Demande.PrixUnitaire,
                    Remise = dossier.Demande.Remise,
                    EstHomologable = dossier.Demande.EstHomologable,

                    Statut = dossier.Demande.Statut != null ? new StatutDto
                    {
                        Id = dossier.Demande.Statut.Id,
                        Code = dossier.Demande.Statut.Code,
                        Libelle = dossier.Demande.Statut.Libelle
                    } : null,

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
                        Extension = doc.Extension,
                        FilePath = $"/api/documents/demande/{doc.Id}/download"
                    }).ToList()
                }
            } : new List<DemandeDto>(),

            Attestations = dossier.Demande != null
                ? dossier.Demande.Attestations
                    .Where(att => att.Donnees != null && att.Donnees.Length > 0)
                    .Select(att => new AttestationDto
                    {
                        Id = att.Id,
                        DateDelivrance = att.DateDelivrance,
                        FilePath = $"/api/documents/certificat/{att.Id}/download"
                    }).ToList()
                : new List<AttestationDto>()

        }).ToList();

        return new DossiersListVm
        {
            Dossiers = dossierDtos,
            Page = request.Parameters.Page,
            TotalPage = (int)Math.Ceiling(totalCount / (double)request.Parameters.TaillePage),
            Recherche = request.Parameters.Recherche
        };
    }
}