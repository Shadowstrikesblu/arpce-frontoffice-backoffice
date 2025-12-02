namespace BackOffice.Application.Features.Stats.Queries.GetUserStats;

public class UserStatsDto
{
    public int NbAdmin { get; set; }
    public int NbRedevableValide { get; set; }
    public int NbRedevable { get; set; }
    public int NbLdapAdmin { get; set; }
    public int NbRedevableParticulier { get; set; }
    public int NbRedevableEntreprise { get; set; }
    public int NbProfile { get; set; }
    public int NbAccess { get; set; }
}