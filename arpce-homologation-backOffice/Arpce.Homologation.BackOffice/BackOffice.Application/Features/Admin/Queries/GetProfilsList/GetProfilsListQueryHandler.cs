using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Admin.Queries.GetProfilsList;

public class GetProfilsListQueryHandler : IRequestHandler<GetProfilsListQuery, List<ProfilListItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetProfilsListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProfilListItemDto>> Handle(GetProfilsListQuery request, CancellationToken cancellationToken)
    {
        // On doit charger les profils et faire des counts sur les relations.
        // Attention : pour les utilisateurs LDAP, il faut une relation explicite ou une 2eme requete.
        // Supposons que AdminProfils a une collection 'Utilisateurs' (AdminUtilisateurs).
        // Pour Acces, on compte dans AdminProfilsAcces.

        var profils = await _context.AdminProfils.AsNoTracking()
            .ToListAsync(cancellationToken);

        var result = new List<ProfilListItemDto>();

        foreach (var p in profils)
        {
            // On fait des requêtes séparées pour les counts pour éviter une requête monstrueuse
            // ou on pourrait utiliser des Include().Count() si configuré.

            var nbAcces = await _context.AdminProfilsAcces.CountAsync(a => a.IdProfil == p.Id, cancellationToken);
            var nbUsers = await _context.AdminUtilisateurs.CountAsync(u => u.IdProfil == p.Id, cancellationToken);
            var nbLdapUsers = await _context.AdminProfilsUtilisateursLDAP.CountAsync(l => l.IdProfil == p.Id, cancellationToken);

            result.Add(new ProfilListItemDto
            {
                Id = p.Id,
                Code = p.Code,
                Libelle = p.Libelle,
                Remarques = p.Remarques,
                UtilisateurCreation = p.UtilisateurCreation,
                DateCreation = p.DateCreation,
                UtilisateurModification = p.UtilisateurModification,
                DateModification = p.DateModification,
                NbAcces = nbAcces,
                Utilisateurs = nbUsers,
                UtilisateursLDAP = nbLdapUsers
            });
        }

        return result;
    }
}