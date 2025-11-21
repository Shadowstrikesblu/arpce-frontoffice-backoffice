namespace BackOffice.Domain.Entities;

/// <summary>
/// Représente le statut d'un dossier ou d'une demande d'homologation.
/// </summary>
public class Statut
{
    /// <summary>
    /// Identifiant unique du statut.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Code court unique du statut 
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Libellé descriptif du statut (ex: "Nouvelle demande", "Validé").
    /// </summary>
    public string Libelle { get; set; } = string.Empty;
}