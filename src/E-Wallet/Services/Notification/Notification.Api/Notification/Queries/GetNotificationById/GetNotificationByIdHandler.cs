using BuildingBlocks.CQRS;
using Notification.Api.Exceptions;
using Notification.Api.Interfaces;

namespace Notification.Api.Notification.Queries.GetNotificationById;

public record GetNotificationByIdQuery(Guid NotificationId) : IQuery<GetNotificationByIdResult>;

public record GetNotificationByIdResult(Entities.Notification Notification);
public class GetNotificationByIdHandler(INotificationRepository notificationRepository)
    : IQueryHandler<GetNotificationByIdQuery, GetNotificationByIdResult>
{
    public async Task<GetNotificationByIdResult> Handle(GetNotificationByIdQuery query, CancellationToken cancellationToken)
    {
        var notification = await notificationRepository.GetByIdAsync(query.NotificationId.ToString(), cancellationToken)
            ?? throw new NotificationNotFoundException(query.NotificationId.ToString());
        return new GetNotificationByIdResult(notification);
    }
}