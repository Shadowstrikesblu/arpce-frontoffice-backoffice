namespace BackOffice.Domain.Entities;

/// <summary>
/// Représente un mode de règlement pour les paiements.
/// </summary>
public class ModeReglement
{
    /// <summary>
    /// Identifiant unique du mode de règlement.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Code court unique du mode de règlement 
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Libellé descriptif du mode de règlement.
    /// </summary>
    public string Libelle { get; set; } = string.Empty;

    /// <summary>
    /// Indique si ce mode de règlement utilise le mobile banking (0=non, 1=oui).
    /// </summary>
    public byte MobileBanking { get; set; } // Respecte le type 'byte' du Front Office.

    /// <summary>
    /// Remarques diverses concernant ce mode de règlement.
    /// </summary>
    public string? Remarques { get; set; }

    /// <summary>
    /// Nom de l'utilisateur qui a créé l'entité.
    /// </summary>
    public string? UtilisateurCreation { get; set; }

    /// <summary>
    /// Date et heure de création de l'entité.
    /// </summary>
    public long? DateCreation { get; set; }

    /// <summary>
    /// Nom de l'utilisateur qui a modifié l'entité pour la dernière fois.
    /// </summary>
    public string? UtilisateurModification { get; set; }

    /// <summary>
    /// Date et heure de la dernière modification de l'entité.
    /// </summary>
    public long? DateModification { get; set; }
}