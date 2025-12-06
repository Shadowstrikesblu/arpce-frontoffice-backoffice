namespace FrontOffice.Application.Features.Dossiers.Queries.GetDossiersFullList
{
    /// <summary>
    /// ViewModel pour la réponse paginée de la liste complète des dossiers.
    /// </summary>
    public class DossiersFullListVm
    {
        public int Page { get; set; }
        public int PageTaille { get; set; }
        public int TotalPage { get; set; }
        public List<DossierFullListItemDto> Dossiers { get; set; } = new();
    }
}
