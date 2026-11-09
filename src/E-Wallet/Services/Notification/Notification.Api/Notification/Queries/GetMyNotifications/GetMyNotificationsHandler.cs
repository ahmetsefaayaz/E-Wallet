using BuildingBlocks.CQRS;
using Notification.Api.Exceptions;
using Notification.Api.Interfaces;

namespace Notification.Api.Notification.Queries.GetMyNotifications;
public record GetMyNotificationsQuery(Guid UserId) : IQuery<GetMyNotificationsResult>;
public record GetMyNotificationsResult(IEnumerable<Entities.Notification> Notifications);
public class GetMyNotificationsQueryHandler(INotificationRepository notificationRepository)
    : IQueryHandler<GetMyNotificationsQuery, GetMyNotificationsResult>
{
    public async Task<GetMyNotificationsResult> Handle(
        GetMyNotificationsQuery query, 
        CancellationToken cancellationToken)
    {
        var notifications = await notificationRepository.GetByUserIdAsync(query.UserId.ToString(), cancellationToken)
                            ?? throw new NotificationNotFoundException();
            
        return new GetMyNotificationsResult(notifications);
    }
}