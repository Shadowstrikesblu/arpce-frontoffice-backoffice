using BackOffice.Application.Common.DTOs;
using BackOffice.Application.Common.Interfaces;
using BackOffice.Application.Features.Admin.Queries.GetAdminUsersList;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BackOffice.Application.Common.Exceptions;

namespace BackOffice.Application.Features.Authentication.Queries.CheckToken;

public class CheckTokenQueryHandler : IRequestHandler<CheckTokenQuery, AdminUserDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CheckTokenQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<AdminUserDto> Handle(CheckTokenQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue) throw new UnauthorizedAccessException("Token invalide.");

        var user = await _context.AdminUtilisateurs
            .AsNoTracking()
            .Include(u => u.UtilisateurType)
            .FirstOrDefaultAsync(u => u.Id == userId.Value, cancellationToken);

        if (user == null) throw new UnauthorizedAccessException("Utilisateur introuvable.");

        var userDto = new AdminUserDto
        {
            Id = user.Id,
            IdProfil = user.IdProfil,
            IdUtilisateurType = user.IdUtilisateurType,

            Compte = user.Compte,
            Nom = user.Nom,
            Prenoms = user.Prenoms,
            ChangementMotPasse = user.ChangementMotPasse,
            Desactive = user.Desactive,
            Remarques = user.Remarques,

            DerniereConnexion = user.DerniereConnexion.FromUnixTimeMilliseconds(),
            UtilisateurCreation = user.UtilisateurCreation,
            DateCreation = user.DateCreation.FromUnixTimeMilliseconds(),
            UtilisateurModification = user.UtilisateurModification,
            DateModification = user.DateModification.FromUnixTimeMilliseconds(),

            TypeUtilisateur = user.UtilisateurType != null ? new AdminUserTypeSimpleDto
            {
                Libelle = user.UtilisateurType.Libelle
            } : null,

            Profil = null
        };

        if (user.IdProfil.HasValue)
        {
            var profil = await _context.AdminProfils
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == user.IdProfil.Value, cancellationToken);

            if (profil != null)
            {
                var accessList = await _context.AdminProfilsAcces
                    .AsNoTracking()
                    .Where(apa => apa.IdProfil == profil.Id)
                    .Include(apa => apa.Access)
                    .ToListAsync(cancellationToken);

                userDto.Profil = new AdminProfilDto
                {
                    Id = profil.Id,
                    Code = profil.Code ?? string.Empty,
                    Libelle = profil.Libelle,
                    Remarques = profil.Remarques,
                    UtilisateurCreation = profil.UtilisateurCreation,

                    DateCreation = profil.DateCreation.FromUnixTimeMilliseconds(),

                    Acces = accessList.Select(a => new AdminProfilAccesDto
                    {
                        IdProfil = a.IdProfil,
                        IdAccess = a.IdAccess,
                        Ajouter = a.Ajouter == 1,
                        Valider = a.Valider == 1,
                        Supprimer = a.Supprimer == 1,
                        Imprimer = a.Imprimer == 1,
                        Access = new AdminAccessDto
                        {
                            Id = a.Access!.Id,
                            Application = a.Access.Application,
                            Groupe = a.Access.Groupe,
                            Libelle = a.Access.Libelle,
                            Page = a.Access.Page,
                            Type = a.Access.Type,
                            Inactif = a.Access.Inactif == 1,
                            Ajouter = a.Access.Ajouter == 1,
                            Valider = a.Access.Valider == 1,
                            Supprimer = a.Access.Supprimer == 1,
                            Imprimer = a.Access.Imprimer == 1
                        }
                    }).ToList()
                };
            }
        }

        return userDto;
    }
}