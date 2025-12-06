using BackOffice.Application.Common.DTOs;
using BackOffice.Application.Common.Interfaces;
using BackOffice.Application.Features.Authentication.Queries.CheckToken;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace BackOffice.Application.Features.Admin.Queries.GetAdminUserDetail;

public class GetAdminUserDetailQueryHandler : IRequestHandler<GetAdminUserDetailQuery, AdminUserDto>
{
    private readonly IApplicationDbContext _context;

    public GetAdminUserDetailQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AdminUserDto> Handle(GetAdminUserDetailQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.AdminUtilisateurs
            .AsNoTracking()
            .Include(u => u.UtilisateurType) 
            .FirstOrDefaultAsync(u => u.Id == request.UtilisateurId, cancellationToken);

        if (user == null)
        {
            throw new Exception($"Utilisateur administrateur avec l'ID '{request.UtilisateurId}' introuvable.");
        }

        // Mapping de base
        var userDto = new AdminUserDto
        {
            Id = user.Id,
            IdProfil = user.IdProfil,
            Compte = user.Compte,
            Nom = user.Nom,
            Prenoms = user.Prenoms,
            ChangementMotPasse = user.ChangementMotPasse,
            Desactive = user.Desactive,
            Remarques = user.Remarques,
            DerniereConnexion = user.DerniereConnexion,
            UtilisateurCreation = user.UtilisateurCreation,
            DateCreation = user.DateCreation,
            UtilisateurModification = user.UtilisateurModification,
            DateModification = user.DateModification,
            IdUtilisateurType = user.IdUtilisateurType,
        };

        // Chargement du profil et des accès
        if (user.IdProfil.HasValue)
        {
            var profil = await _context.AdminProfils.FindAsync(user.IdProfil.Value);
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
                    DateCreation = profil.DateCreation,
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