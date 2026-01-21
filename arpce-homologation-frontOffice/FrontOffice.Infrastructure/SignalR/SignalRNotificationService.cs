using FrontOffice.Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace FrontOffice.Infrastructure.SignalR;

/// <summary>
/// Service de notification du Front Office. Son rôle est de notifier le Back Office.
/// Pour cela, il devra appeler une API du Back Office, ou (pour une base partagée) insérer dans la table notifications.
/// </summary>
public class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IApplicationDbContext _context; 

    public SignalRNotificationService(IHubContext<NotificationHub> hubContext, IApplicationDbContext context)
    {
        _hubContext = hubContext;
        _context = context;
    }

    // Notifie le client connecté au Front Office
    public async Task SendToUserAsync(string userId, string title, string message, string type = "Info", string? targetUrl = null, string? entityId = null)
    {
        var payload = new { Title = title, Message = message, Type = type, TargetUrl = targetUrl, EntityId = entityId };
        await _hubContext.Clients.Group(userId).SendAsync("ReceiveNotification", payload);
    }

    // Notifie les agents du Back Office en écrivant dans la table partagée
    public async Task SendToGroupAsync(string groupName, string title, string message, string type = "Info", string? targetUrl = null, string? entityId = null)
    {
        // On insère une notification dans la table que le Back Office lit
        var notification = new Domain.Entities.Notification
        {
            Id = Guid.NewGuid(),
            ProfilCode = groupName,
            Title = title,
            Message = message,
            Type = type,
            TargetUrl = targetUrl,
            EntityId = entityId,
            DateEnvoi = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            UtilisateurCreation = "SYSTEM_FRONT_OFFICE"
        };
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(default);
        // Note : On ne fait pas de HubContext.Send ici car on notifie le BACK OFFICE.
        // Le back office doit avoir son propre Hub pour écouter ces nouvelles entrées.
    }

    // Non utilisé dans ce contexte, mais requis par l'interface
    public Task SendToAllAsync(string title, string message, string type = "Info", string? targetUrl = null, string? entityId = null)
    {
        // On peut notifier tous les clients du Front Office si besoin
        var payload = new { Title = title, Message = message, Type = type, TargetUrl = targetUrl, EntityId = entityId };
        return _hubContext.Clients.All.SendAsync("ReceiveNotification", payload);
    }
}