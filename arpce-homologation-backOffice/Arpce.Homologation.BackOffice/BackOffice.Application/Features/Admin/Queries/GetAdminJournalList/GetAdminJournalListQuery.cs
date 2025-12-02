using MediatR;

namespace BackOffice.Application.Features.Admin.Queries.GetAdminJournalList;

public class GetAdminJournalListQuery : IRequest<JournalListVm>
{
    public int Page { get; set; } = 1;
    public int PageTaille { get; set; } = 10;
    public string? Recherche { get; set; }
    public string? Ordre { get; set; }
}