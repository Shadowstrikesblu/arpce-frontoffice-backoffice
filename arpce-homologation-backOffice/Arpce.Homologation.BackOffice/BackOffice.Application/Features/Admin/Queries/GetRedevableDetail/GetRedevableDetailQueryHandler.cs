using BackOffice.Application.Common.Exceptions;
using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
// AJOUTEZ CETTE LIGNE SI ELLE N'Y EST PAS
using BackOffice.Application.Common;

namespace BackOffice.Application.Features.Admin.Queries.GetRedevableDetail;

public class GetRedevableDetailQueryHandler : IRequestHandler<GetRedevableDetailQuery, RedevableDetailDto>
{
    private readonly IApplicationDbContext _context;

    public GetRedevableDetailQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RedevableDetailDto> Handle(GetRedevableDetailQuery request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients.AsNoTracking()
            .Include(c => c.Dossiers)
                .ThenInclude(d => d.Statut)
            .Include(c => c.Dossiers)
                .ThenInclude(d => d.Demandes)
            .FirstOrDefaultAsync(c => c.Id == request.UtilisateurId, cancellationToken);

        if (client == null)
        {
            throw new Exception($"Redevable avec l'ID '{request.UtilisateurId}' introuvable.");
        }

        return new RedevableDetailDto
        {
            Code = client.Code,
            RaisonSociale = client.RaisonSociale,
            RegistreCommerce = client.RegistreCommerce,
            Desactive = client.Desactive == 1,
            ContactNom = client.ContactNom,
            ContactTelephone = client.ContactTelephone,
            ContactFonction = client.ContactFonction,
            Email = client.Email,
            Adresse = client.Adresse,
            Bp = client.Bp,
            Ville = client.Ville,
            Pays = client.Pays,
            Remarques = client.Remarques,
            UtilisateurCreation = client.UtilisateurCreation,

            // --- CORRECTIONS ---
            DateCreation = client.DateCreation.FromUnixTimeMilliseconds(),
            UtilisateurModification = client.UtilisateurModification,
            DateModification = client.DateModification.FromUnixTimeMilliseconds(),

            Dossiers = client.Dossiers.Select(d => new DossierRedevableDto
            {
                Id = d.Id,
                Numero = d.Numero,
                Libelle = d.Libelle,
                DateOuverture = d.DateOuverture.FromUnixTimeMilliseconds(),
                StatutLibelle = d.Statut?.Libelle ?? "Inconnu",
                NombreEquipements = d.Demandes.Count
            }).ToList()
        };
    }
}