using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace BackOffice.Infrastructure.SignalR;

/// <summary>
/// Hub SignalR sécurisé.
/// Modifié pour respecter la demande Front : "Broadcast avec filtrage côté client".
/// On ne gère plus les groupes de profils ici.
/// </summary>
[Authorize]
public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var user = Context.User;

        if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                         user.FindFirst("sub")?.Value; 

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            }
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}