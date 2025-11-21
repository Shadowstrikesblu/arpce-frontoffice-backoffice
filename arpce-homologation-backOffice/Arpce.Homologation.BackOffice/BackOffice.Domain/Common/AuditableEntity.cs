namespace BackOffice.Domain.Common;

/// <summary>
/// Classe de base abstraite pour les entités nécessitant des champs d'audit.
/// </summary>
public abstract class AuditableEntity
{
    /// <summary>
    /// Identifiant unique de l'entité.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid(); 

    /// <summary>
    /// Nom de l'utilisateur qui a créé l'entité.
    /// </summary>
    public string? UtilisateurCreation { get; set; }

    /// <summary>
    /// Date et heure de création de l'entité.
    /// </summary>
    public DateTime? DateCreation { get; set; }

    /// <summary>
    /// Nom de l'utilisateur qui a modifié l'entité pour la dernière fois.
    /// </summary>
    public string? UtilisateurModification { get; set; }

    /// <summary>
    /// Date et heure de la dernière modification de l'entité.
    /// </summary>
    public DateTime? DateModification { get; set; }
}