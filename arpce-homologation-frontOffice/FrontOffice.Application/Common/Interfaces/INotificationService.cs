using System.Threading.Tasks;

namespace FrontOffice.Application.Common.Interfaces;

public interface INotificationService
{
    Task SendToUserAsync(string userId, string title, string message, string type = "Info", string? targetUrl = null, string? entityId = null);
    Task SendToGroupAsync(string groupName, string title, string message, string type = "Info", string? targetUrl = null, string? entityId = null);
    Task SendToAllAsync(string title, string message, string type = "Info", string? targetUrl = null, string? entityId = null);
}