using BuildingBlocks.CQRS;
using Notification.Api.Exceptions;
using Notification.Api.Interfaces;

namespace Notification.Api.Notification.Queries.GetNotificationById;

public record GetNotificationByIdQuery(Guid NotificationId, Guid CurrentUserId, bool IsNotificationDepartment) : IQuery<GetNotificationByIdResult>;

public record GetNotificationByIdResult(Entities.Notification Notification);
public class GetNotificationByIdHandler(INotificationRepository notificationRepository)
    : IQueryHandler<GetNotificationByIdQuery, GetNotificationByIdResult>
{
    public async Task<GetNotificationByIdResult> Handle(GetNotificationByIdQuery query, CancellationToken cancellationToken)
    {
        var notification = await notificationRepository.GetByIdAsync(query.NotificationId.ToString(), cancellationToken)
            ?? throw new NotificationNotFoundException(query.NotificationId.ToString());
        if (!query.IsNotificationDepartment && notification.UserId != query.CurrentUserId)
            throw new BuildingBlocks.Exceptions.ForbiddenAccessException("You are not authorized to view this notification.");
        return new GetNotificationByIdResult(notification);
    }
}