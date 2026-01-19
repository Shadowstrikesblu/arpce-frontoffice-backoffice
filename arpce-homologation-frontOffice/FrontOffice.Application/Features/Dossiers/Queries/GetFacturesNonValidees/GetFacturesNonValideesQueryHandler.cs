using FrontOffice.Application.Common.DTOs;
using FrontOffice.Application.Common.Interfaces;
using FrontOffice.Application.Features.Dossiers.Queries.GetDossiersList;
using FrontOffice.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FrontOffice.Application.Features.Dossiers.Queries.GetFacturesNonValidees;

public class GetFacturesNonValideesQueryHandler : IRequestHandler<GetFacturesNonValideesQuery, DossiersListVm>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetFacturesNonValideesQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _currentUserService = currentUserService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<DossiersListVm> Handle(GetFacturesNonValideesQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue) throw new UnauthorizedAccessException("Utilisateur non authentifié.");

        const string statutCible = "EnPaiement";

        
        const byte typeFacture = 2;

        var query = _context.Dossiers.AsNoTracking()
            .Where(d => d.IdClient == userId.Value && d.Statut.Code == statutCible);

        if (!string.IsNullOrWhiteSpace(request.Parameters.Recherche))
        {
            var term = request.Parameters.Recherche.Trim().ToLower();
            query = query.Where(d => d.Numero.ToLower().Contains(term) || d.Libelle.ToLower().Contains(term));
        }

        query = query.OrderByDescending(d => d.DateOuverture);

        var totalCount = await query.CountAsync(cancellationToken);

        var dossiers = await query
            .Include(d => d.Statut)
            .Include(d => d.DocumentsDossiers) 
            .Skip((request.Parameters.Page - 1) * request.Parameters.TaillePage)
            .Take(request.Parameters.TaillePage)
            .ToListAsync(cancellationToken);

        var requestContext = _httpContextAccessor.HttpContext!.Request;
        var baseUrl = $"{requestContext.Scheme}://{requestContext.Host}";

        var dtos = dossiers.Select(d => new DossierListItemDto
        {
            Id = d.Id,
            Numero = d.Numero,
            Libelle = d.Libelle,
            DateOuverture = d.DateOuverture,
            Statut = d.Statut != null ? new StatutDto { Id = d.Statut.Id, Code = d.Statut.Code, Libelle = d.Statut.Libelle } : null,

            Documents = d.DocumentsDossiers
                .Where(doc => doc.Type == typeFacture)
                .Select(doc => new DocumentDossierDto
                {
                    Id = doc.Id,
                    Nom = doc.Nom,
                    Type = doc.Type,
                    Extension = doc.Extension,
                    FilePath = $"/api/documents/dossier/{doc.Id}/download"
                }).ToList(),

            Devis = new List<DevisDto>()

        }).ToList();

        return new DossiersListVm
        {
            Dossiers = dtos,
            Page = request.Parameters.Page,
            TotalPage = (int)Math.Ceiling(totalCount / (double)request.Parameters.TaillePage),
            Recherche = request.Parameters.Recherche
        };
    }
}