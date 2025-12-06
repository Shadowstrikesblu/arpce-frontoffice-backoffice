using BackOffice.Application.Common.DTOs;
using MediatR;

namespace BackOffice.Application.Features.Admin.Queries.GetAdminUserDetail;

/// <summary>
/// Requête pour obtenir les détails complets d'un administrateur.
/// Retourne le même DTO complet que 'CheckToken' (AdminUserDto).
/// </summary>
public class GetAdminUserDetailQuery : IRequest<AdminUserDto>
{
    public Guid UtilisateurId { get; set; }

    public GetAdminUserDetailQuery(Guid utilisateurId)
    {
        UtilisateurId = utilisateurId;
    }
}