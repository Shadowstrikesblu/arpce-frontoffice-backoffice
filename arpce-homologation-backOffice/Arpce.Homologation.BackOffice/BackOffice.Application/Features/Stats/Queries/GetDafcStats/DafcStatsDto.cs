namespace BackOffice.Application.Features.Stats.Queries.GetDafcStats;

public class DafcStatsDto
{
    public int NbFactureExpire { get; set; }
    public int NbFactureEnAttente { get; set; }
    public int NbFacturePaye { get; set; }
    public int NbDevisValide { get; set; }
    public int NbDevisRefuse { get; set; }
    public int NbDevisEnAttente { get; set; }
}