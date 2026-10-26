using BuildingBlocks.CQRS;
using Notification.Api.Exceptions;
using Notification.Api.Interfaces;

namespace Notification.Api.Notification.Queries.GetNotificationsByUserId;

public record GetNotificationsByUserIdQuery(Guid UserId): IQuery<GetNotificationsByUserIdResult>;
public record GetNotificationsByUserIdResult(IEnumerable<Entities.Notification> Notifications);

public class GetNotificationsByUserIdHandler(INotificationRepository notificationRepository)
    : IQueryHandler<GetNotificationsByUserIdQuery, GetNotificationsByUserIdResult>
{
    public async Task<GetNotificationsByUserIdResult> Handle(GetNotificationsByUserIdQuery query, CancellationToken cancellationToken)
    {
        var notification = await notificationRepository.GetByUserIdAsync(query.UserId.ToString(), cancellationToken)
            ?? throw new NotificationNotFoundException();
        return new GetNotificationsByUserIdResult(notification);
    }
}