namespace BackOffice.Application.Features.Admin.Queries.GetAdminJournalList
{
    public class JournalListVm
    {
        public int Page { get; set; }
        public int PageTaille { get; set; }
        public int TotalPage { get; set; }
        public List<JournalItemDto> Journal { get; set; } = new();
    }
}
