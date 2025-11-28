using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

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

        // Récupérer l'utilisateur (sans les includes pour l'instant, car on va charger manuellement)
        var user = await _context.AdminUtilisateurs
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId.Value, cancellationToken);

        if (user == null) throw new UnauthorizedAccessException("Utilisateur introuvable.");

        // Mapping complet des champs simples de l'utilisateur
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
            Profil = null // On va le remplir juste après
        };

        // Chargement du profil et des accès si l'utilisateur a un profil
        if (user.IdProfil.HasValue)
        {
            var profil = await _context.AdminProfils
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == user.IdProfil.Value, cancellationToken);

            if (profil != null)
            {
                // Récupérer les liaisons Profil-Acces
                var accessList = await _context.AdminProfilsAcces
                    .AsNoTracking()
                    .Where(apa => apa.IdProfil == profil.Id)
                    .Include(apa => apa.Access) // Charger l'objet Access lié
                    .ToListAsync(cancellationToken);

                // Mapping du Profil
                userDto.Profil = new AdminProfilDto
                {
                    Id = profil.Id,
                    Code = profil.Code ?? string.Empty,
                    Libelle = profil.Libelle,
                    Remarques = profil.Remarques,
                    UtilisateurCreation = profil.UtilisateurCreation,
                    DateCreation = profil.DateCreation,

                    // Mapping de la liste des accès
                    Acces = accessList.Select(a => new AdminProfilAccesDto
                    {
                        IdProfil = a.IdProfil,
                        IdAccess = a.IdAccess,
                        // Conversion byte? -> bool (1 = true, 0 ou null = false)
                        Ajouter = a.Ajouter == 1,
                        Valider = a.Valider == 1,
                        Supprimer = a.Supprimer == 1,
                        Imprimer = a.Imprimer == 1,

                        // Mapping de l'objet Access imbriqué
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