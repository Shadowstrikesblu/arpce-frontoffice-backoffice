namespace BackOffice.Application.Features.Admin.Queries.GetProfilsList
{
    public class ProfilsListVm
    {
        public int Page { get; set; }
        public int PageTaille { get; set; }
        public int TotalPage { get; set; }
        public List<ProfilFullDto> Profils { get; set; } = new();
    }
}
