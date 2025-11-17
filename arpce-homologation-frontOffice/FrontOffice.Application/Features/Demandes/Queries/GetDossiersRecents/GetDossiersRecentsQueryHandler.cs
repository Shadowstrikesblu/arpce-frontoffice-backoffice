using FrontOffice.Application.Common.DTOs; 
using FrontOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FrontOffice.Application.Features.Demandes.Queries.GetDossiersRecents;

public class GetDossiersRecentsQueryHandler : IRequestHandler<GetDossiersRecentsQuery, List<DossierRecentItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetDossiersRecentsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<DossierRecentItemDto>> Handle(GetDossiersRecentsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("Utilisateur non authentifié.");
        }

        var dossiersRecents = await _context.Dossiers
            .Where(d => d.IdClient == userId.Value)
            .Include(d => d.Statut)
            .Include(d => d.Demandes)
            .Select(d => new DossierRecentItemDto
            {
                Id = d.Id,
                IdClient = d.IdClient,
                IdStatut = d.IdStatut,
                IdModeReglement = d.IdModeReglement,
                DateOuverture = d.DateOuverture,
                Numero = d.Numero,
                Libelle = d.Libelle,
                Statut = d.Statut != null ? new StatutDto
                {
                    Id = d.Statut.Id,
                    Code = d.Statut.Code,
                    Libelle = d.Statut.Libelle
                } : null,
                Demandes = d.Demandes.Select(dem => new DemandeDto
                {
                    Id = dem.Id,
                    IdDossier = dem.IdDossier,
                    NumeroDemande = dem.NumeroDemande,
                    Equipement = dem.Equipement,
                    Modele = dem.Modele,
                    Marque = dem.Marque
                }).ToList()
            })
            .ToListAsync(cancellationToken);

        return dossiersRecents;
    }
}