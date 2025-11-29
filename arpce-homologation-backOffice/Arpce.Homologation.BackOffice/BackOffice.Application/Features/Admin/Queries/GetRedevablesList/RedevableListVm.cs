namespace BackOffice.Application.Features.Admin.Queries.GetRedevablesList
{
    public class RedevableListVm
    {
        public int Page { get; set; }
        public int PageTaille { get; set; }
        public int TotalPage { get; set; }
        public List<RedevableListItemDto> Utilisateur { get; set; } = new();
    }
}
