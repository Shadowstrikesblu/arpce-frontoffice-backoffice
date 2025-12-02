using BackOffice.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Stats.Queries.GetUserStats;

public class GetUserStatsQueryHandler : IRequestHandler<GetUserStatsQuery, UserStatsDto>
{
    private readonly IApplicationDbContext _context;

    public GetUserStatsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserStatsDto> Handle(GetUserStatsQuery request, CancellationToken cancellationToken)
    {
        // Exécution séquentielle ou parallèle des requêtes de comptage
        var nbAdmin = await _context.AdminUtilisateurs.CountAsync(cancellationToken);

        var nbRedevable = await _context.Clients.CountAsync(cancellationToken);

        // NiveauValidation == 2 signifie validé par l'admin
        var nbRedevableValide = await _context.Clients.CountAsync(c => c.NiveauValidation == 2, cancellationToken);

        var nbLdapAdmin = await _context.AdminProfilsUtilisateursLDAP.CountAsync(cancellationToken);

        var nbRedevableParticulier = await _context.Clients.CountAsync(c => c.TypeClient == "Particulier", cancellationToken);

        var nbRedevableEntreprise = await _context.Clients.CountAsync(c => c.TypeClient == "Entreprise", cancellationToken);

        var nbProfile = await _context.AdminProfils.CountAsync(cancellationToken);

        var nbAccess = await _context.AdminAccesses.CountAsync(cancellationToken);

        return new UserStatsDto
        {
            NbAdmin = nbAdmin,
            NbRedevable = nbRedevable,
            NbRedevableValide = nbRedevableValide,
            NbLdapAdmin = nbLdapAdmin,
            NbRedevableParticulier = nbRedevableParticulier,
            NbRedevableEntreprise = nbRedevableEntreprise,
            NbProfile = nbProfile,
            NbAccess = nbAccess
        };
    }
}