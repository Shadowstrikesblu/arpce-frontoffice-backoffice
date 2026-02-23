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

    public GetNotificationHistoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<NotificationListVm> Handle(GetNotificationHistoryQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Notification> query = _context.Notifications
            .AsNoTracking()
            .OrderByDescending(n => n.DateEnvoi);

        // Calcul du total de toutes les notifications en base
        var totalCount = await query.CountAsync(cancellationToken);

        // Récupération paginée de l'historique complet
        var items = await query
            .Skip((request.Page - 1) * request.PageTaille)
            .Take(request.PageTaille)
            .ToListAsync(cancellationToken);

        // Mapping vers le DTO incluant les champs de ciblage
        var dtos = items.Select(n => new NotificationDto
        {
            Id = n.Id,
            Title = n.Title,
            Message = n.Message,
            Type = n.Type,
            TargetUrl = n.TargetUrl,
            EntityId = n.EntityId,
            IsRead = n.IsRead,
            DateEnvoi = n.DateEnvoi.FromUnixTimeMilliseconds(),

            // mappe les nouveaux champs requis
            UserId = n.UserId,
            ProfilCode = n.ProfilCode,
            IsBroadcast = n.IsBroadcast
        }).ToList();

        // Retour du résultat
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