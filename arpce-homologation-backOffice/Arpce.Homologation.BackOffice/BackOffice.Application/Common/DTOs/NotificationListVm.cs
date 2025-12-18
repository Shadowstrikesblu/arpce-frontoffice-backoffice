namespace BackOffice.Application.Common.DTOs;

public class NotificationListVm
{
    public List<NotificationDto> Notifications { get; set; } = new();
    public int Page { get; set; }
    public int PageTaille { get; set; }
    public int TotalCount { get; set; }
    public int TotalPage { get; set; }
}