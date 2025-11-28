namespace BackOffice.Application.Features.Documents.Queries.GetFacturesList
{
    public class DevisInfoInDocDto
    {
        public DateTime Date { get; set; }
        public decimal MontantEtude { get; set; }
        public decimal? MontantHomologation { get; set; }
        public decimal? MontantControle { get; set; }
        public byte? PaiementOk { get; set; }
        public string? PaiementMobileId { get; set; }
    }
}
