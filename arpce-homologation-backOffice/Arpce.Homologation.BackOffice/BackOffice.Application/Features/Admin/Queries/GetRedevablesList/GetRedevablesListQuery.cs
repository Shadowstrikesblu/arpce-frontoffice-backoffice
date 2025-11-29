using MediatR;

namespace BackOffice.Application.Features.Admin.Queries.GetRedevablesList
{
    public class GetRedevablesListQuery : IRequest<RedevableListVm>
    {
        public int Page { get; set; } = 1;
        public int PageTaille { get; set; } = 10;
        public string? Recherche { get; set; }
        public string? Ordre { get; set; }
    }
}
