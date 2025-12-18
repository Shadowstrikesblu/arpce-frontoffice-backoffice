using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using Microsoft.AspNetCore.SignalR;

namespace BackOffice.Infrastructure.SignalR;

public class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IApplicationDbContext _context;

    public SignalRNotificationService(IHubContext<NotificationHub> hubContext, IApplicationDbContext context)
    {
        _hubContext = hubContext;
        _context = context;
    }

    public async Task SendToUserAsync(string userId, string title, string message, string type = "Info", string? targetUrl = null, string? entityId = null)
    {
        // Persistance
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = Guid.Parse(userId),
            ProfilCode = null,
            IsBroadcast = false,
            Title = title,
            Message = message,
            Type = type,
            TargetUrl = targetUrl,
            EntityId = entityId,
            DateEnvoi = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            IsRead = false,
            DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            UtilisateurCreation = "SYSTEM_NOTIF"
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(default);

        // Envoi ciblé via le groupe UserId
        var payload = new
        {
            Title = title,
            Message = message,
            Type = type,
            TargetUrl = targetUrl,
            EntityId = entityId,
            TargetProfile = (string?)null 
        };

        await _hubContext.Clients.Group(userId).SendAsync("ReceiveNotification", payload);
    }

    public async Task SendToGroupAsync(string profilCode, string title, string message, string type = "Info", string? targetUrl = null, string? entityId = null)
    {
        // Persistance 
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = null,
            ProfilCode = profilCode,
            IsBroadcast = false, 
            Title = title,
            Message = message,
            Type = type,
            TargetUrl = targetUrl,
            EntityId = entityId,
            DateEnvoi = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            IsRead = false,
            DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            UtilisateurCreation = "SYSTEM_NOTIF"
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(default);

        // Envoi BROADCAST (À tout le monde) avec la propriété TargetProfile
        // Le front va recevoir ça, vérifier "Est-ce que j'ai le profil 'profilCode' ?"
        // Si oui => Affiche. Si non => Zappe.
        var payload = new
        {
            Title = title,
            Message = message,
            Type = type,
            TargetUrl = targetUrl,
            EntityId = entityId,
            TargetProfile = profilCode 
        };

        await _hubContext.Clients.All.SendAsync("ReceiveNotification", payload);
    }

    public async Task SendToAllAsync(string title, string message, string type = "Info", string? targetUrl = null, string? entityId = null)
    {
        // Persistance
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = null,
            ProfilCode = null,
            IsBroadcast = true,
            Title = title,
            Message = message,
            Type = type,
            TargetUrl = targetUrl,
            EntityId = entityId,
            DateEnvoi = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            IsRead = false,
            DateCreation = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            UtilisateurCreation = "SYSTEM_NOTIF"
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(default);

        // Envoi
        var payload = new
        {
            Title = title,
            Message = message,
            Type = type,
            TargetUrl = targetUrl,
            EntityId = entityId,
            TargetProfile = "ALL" 
        };

        await _hubContext.Clients.All.SendAsync("ReceiveNotification", payload);
    }
}