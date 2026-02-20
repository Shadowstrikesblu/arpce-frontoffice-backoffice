using BackOffice.Application.Common; 
using BackOffice.Application.Common.Exceptions;
using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
                .ThenInclude(d => d.Demande) 
            .FirstOrDefaultAsync(c => c.Id == request.UtilisateurId, cancellationToken);

        if (client == null) throw new Exception("Redevable introuvable.");

        return new RedevableDetailDto
        {
            Code = client.Code,
            RaisonSociale = client.RaisonSociale,
            RegistreCommerce = client.RegistreCommerce,
            Desactive = client.Desactive == 1,
            Email = client.Email,

            DateCreation = client.DateCreation.FromUnixTimeMilliseconds(),
            DateModification = client.DateModification.FromUnixTimeMilliseconds(),

            Dossiers = client.Dossiers.Select(d => new DossierRedevableDto
            {
                Id = d.Id,
                Numero = d.Numero,
                Libelle = d.Libelle,
                DateOuverture = d.DateOuverture.FromUnixTimeMilliseconds(),
                StatutLibelle = d.Statut?.Libelle ?? "Inconnu",
                NombreEquipements = d.Demande != null ? 1 : 0
            }).ToList()
        };
    }
}