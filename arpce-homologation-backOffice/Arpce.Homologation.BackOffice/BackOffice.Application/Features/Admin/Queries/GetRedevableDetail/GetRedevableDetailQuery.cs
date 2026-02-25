using BackOffice.Application.Common.DTOs;
using MediatR;

namespace BackOffice.Application.Features.Admin.Queries.GetRedevableDetail
{
    public class GetRedevableDetailQuery : IRequest<RedevableDetailDto>
    {
        public Guid UtilisateurId { get; set; }

        public GetRedevableDetailQuery(Guid utilisateurId)
        {
            UtilisateurId = utilisateurId;
        }
    }
}
