using BackOffice.Application.Common.DTOs;
using BackOffice.Application.Common.Exceptions;
using BackOffice.Application.Common.Interfaces;
using BackOffice.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Features.Notifications.Queries.GetNotificationHistory;

public class GetNotificationHistoryQuery : IRequest<NotificationListVm>
{
    public int Page { get; set; } = 1;
    public int PageTaille { get; set; } = 10;
}

public class GetNotificationHistoryQueryHandler : IRequestHandler<GetNotificationHistoryQuery, NotificationListVm>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetNotificationHistoryQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<NotificationListVm> Handle(GetNotificationHistoryQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        // On récupère le profil de l'utilisateur pour inclure les notifs de groupe
        var user = await _context.AdminUtilisateurs
            .Include(u => u.Profil)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        string? profilCode = user?.Profil?.Code;

        IQueryable<Notification> query = _context.Notifications
            .AsNoTracking()
            .Where(n =>
                (n.UserId == userId) ||
                (profilCode != null && n.ProfilCode == profilCode) ||
                (n.IsBroadcast == true) 
            )
            .OrderByDescending(n => n.DateEnvoi);

        // Pagination
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageTaille)
            .Take(request.PageTaille)
            .ToListAsync(cancellationToken);

        var dtos = items.Select(n => new NotificationDto
        {
            Id = n.Id,
            Title = n.Title,
            Message = n.Message,
            Type = n.Type,
            TargetUrl = n.TargetUrl,
            EntityId = n.EntityId,
            IsRead = n.IsRead,
            DateEnvoi = n.DateEnvoi.FromUnixTimeMilliseconds()
        }).ToList();

        return new NotificationListVm
        {
            Notifications = dtos,
            Page = request.Page,
            PageTaille = request.PageTaille,
            TotalCount = totalCount,
            TotalPage = (int)Math.Ceiling(totalCount / (double)request.PageTaille)
        };
    }
}