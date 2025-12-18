using System;

namespace BackOffice.Application.Common.DTOs;

public class NotificationDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? TargetUrl { get; set; }
    public string? EntityId { get; set; }
    public bool IsRead { get; set; }
    public DateTime DateEnvoi { get; set; }
}