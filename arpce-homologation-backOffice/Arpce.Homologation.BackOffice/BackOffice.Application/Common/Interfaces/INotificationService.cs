using BackOffice.Application.Common.Models;

namespace BackOffice.Application.Common.Interfaces;

/// <summary>
/// Service de gestion des notifications (Temps réel + Persistance).
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Notifie un utilisateur spécifique.
    /// </summary>
    Task SendToUserAsync(string userId, string title, string message, string type = "Info", string? targetUrl = null, string? entityId = null);

    /// <summary>
    /// Notifie tous les utilisateurs connectés.
    /// </summary>
    Task SendToAllAsync(string title, string message, string type = "Info", string? targetUrl = null, string? entityId = null);

    /// <summary>
    /// Notifie un groupe (Profil) spécifique.
    /// </summary>
    Task SendToGroupAsync(string profilCode, string title, string message, string type = "Info", string? targetUrl = null, string? entityId = null);
    Task SendEventNotificationAsync(NotificationEvent eventType, Guid entityId);
}