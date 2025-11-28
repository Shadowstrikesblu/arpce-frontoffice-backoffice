using BackOffice.Application.Common.DTOs.Documents;
using MediatR;


namespace BackOffice.Application.Features.Documents.Queries.GetFacturesList
{
    public class GetFacturesListQuery : IRequest<DocumentListVm>
    {
        public int Page { get; set; } = 1;
        public int PageTaille { get; set; } = 10;
        public string? Recherche { get; set; }
        public string? Ordre { get; set; } // 'asc' | 'desc'

    }
}
