public class ClientDto
{
    public Guid Id { get; set; }
    public string RaisonSociale { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Adresse { get; set; }
    public string? Ville { get; set; }
    public string? Pays { get; set; }
    public string? ContactNom { get; set; }
    public string? ContactTelephone { get; set; }
    public string? Bp { get; set; }
}