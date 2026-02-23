using BackOffice.Application.Common.DTOs;
using BackOffice.Application.Common.Exceptions;
using BackOffice.Application.Common.Interfaces;
using BackOffice.Application.Features.Dossiers.Queries.GetDossiersList;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Dossiers.Queries.GetMyDossiers;

public class GetMyDossiersQueryHandler : IRequestHandler<GetMyDossiersQuery, DossiersListVm>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetMyDossiersQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<DossiersListVm> Handle(GetMyDossiersQuery request, CancellationToken cancellationToken)
    {
        var agentId = _currentUserService.UserId;
        if (!agentId.HasValue) throw new UnauthorizedAccessException();

        var query = _context.Dossiers.AsNoTracking().Where(d => d.IdAgentInstructeur == agentId.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var dossiersPaged = await query
            .Include(d => d.Client)
            .Include(d => d.Statut)
            .Include(d => d.Demande) // Singulier
            .Skip((request.Parameters.Page - 1) * request.Parameters.TaillePage)
            .Take(request.Parameters.TaillePage)
            .ToListAsync(cancellationToken);

        var dossierDtos = dossiersPaged.Select(dossier => new DossierListItemDto
        {
            Id = dossier.Id,
            DateOuverture = dossier.DateOuverture.FromUnixTimeMilliseconds(),
            Numero = dossier.Numero,
            Libelle = dossier.Libelle,
            Client = dossier.Client != null ? new ClientDto { Id = dossier.Client.Id, RaisonSociale = dossier.Client.RaisonSociale } : null,
            Statut = dossier.Statut != null ? new StatutDto { Id = dossier.Statut.Id, Code = dossier.Statut.Code, Libelle = dossier.Statut.Libelle } : null,

            // Mapping 1:1
            Demande = dossier.Demande != null ? new DemandeDto
            {
                Id = dossier.Demande.Id,
                Equipement = dossier.Demande.Equipement,
                Modele = dossier.Demande.Modele
            } : null
        }).ToList();

        return new DossiersListVm
        {
            Dossiers = dossierDtos,
            Page = request.Parameters.Page,
            PageTaille = request.Parameters.TaillePage,
            TotalPage = (int)Math.Ceiling(totalCount / (double)request.Parameters.TaillePage)
        };
    }
}