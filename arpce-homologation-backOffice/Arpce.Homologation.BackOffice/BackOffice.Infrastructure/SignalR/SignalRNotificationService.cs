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
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = Guid.Parse(userId),
            Title = title,
            Message = message,
            Type = type,
            TargetUrl = targetUrl,
            EntityId = entityId,
            DateEnvoi = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            UtilisateurCreation = "SYSTEM_NOTIF"
        };
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(default);

        var payload = new { Title = title, Message = message, Type = type, TargetUrl = targetUrl, EntityId = entityId };
        await _hubContext.Clients.Group(userId).SendAsync("ReceiveNotification", payload);
    }

    public async Task SendToGroupAsync(string groupName, string title, string message, string type = "Info", string? targetUrl = null, string? entityId = null)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            ProfilCode = groupName,
            Title = title,
            Message = message,
            Type = type,
            TargetUrl = targetUrl,
            EntityId = entityId,
            DateEnvoi = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            UtilisateurCreation = "SYSTEM_NOTIF"
        };
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(default);

        var payload = new { Title = title, Message = message, Type = type, TargetUrl = targetUrl, EntityId = entityId };
        await _hubContext.Clients.Group(groupName).SendAsync("ReceiveNotification", payload);
    }

    public async Task SendToAllAsync(string title, string message, string type = "Info", string? targetUrl = null, string? entityId = null)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            IsBroadcast = true,
            Title = title,
            Message = message,
            Type = type,
            TargetUrl = targetUrl,
            EntityId = entityId,
            DateEnvoi = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            UtilisateurCreation = "SYSTEM_NOTIF"
        };
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(default);

        var payload = new { Title = title, Message = message, Type = type, TargetUrl = targetUrl, EntityId = entityId };
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", payload);
    }
}