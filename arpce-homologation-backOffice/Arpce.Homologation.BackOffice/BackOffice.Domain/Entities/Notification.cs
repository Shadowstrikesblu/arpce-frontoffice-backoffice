using BackOffice.Domain.Common;

namespace BackOffice.Domain.Entities;

/// <summary>
/// Représente une notification envoyée à un utilisateur ou un groupe.
/// Stockée en base pour l'historique.
/// </summary>
public class Notification : AuditableEntity
{
    public Guid Id { get; set; }

    // Contenu
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "Info"; 

    // Navigation / Action
    public string? TargetUrl { get; set; }
    public string? EntityId { get; set; } 

    // Ciblage
    public Guid? UserId { get; set; } 
    public string? ProfilCode { get; set; } 

    /// <summary>
    /// Indique si c'est une notification envoyée à tout le monde.
    /// </summary>
    public bool IsBroadcast { get; set; } = false;

    // État
    public bool IsRead { get; set; } = false;
    public long DateEnvoi { get; set; }

    public string Canal { get; set; } = "SYSTEM"; 
    public string? StatutEnvoi { get; set; }
}