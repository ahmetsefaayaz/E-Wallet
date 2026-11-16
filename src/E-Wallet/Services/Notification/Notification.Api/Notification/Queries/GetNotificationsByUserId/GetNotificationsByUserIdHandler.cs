using BuildingBlocks.CQRS;
using Notification.Api.Exceptions;
using Notification.Api.Interfaces;

namespace Notification.Api.Notification.Queries.GetNotificationsByUserId;

public record GetNotificationsByUserIdQuery(Guid UserId, Guid CurrentUserId, bool IsNotificationDepartment): IQuery<GetNotificationsByUserIdResult>;
public record GetNotificationsByUserIdResult(IEnumerable<Entities.Notification> Notifications);

internal sealed class GetNotificationsByUserIdHandler(INotificationRepository notificationRepository)
    : IQueryHandler<GetNotificationsByUserIdQuery, GetNotificationsByUserIdResult>
{
    public async Task<GetNotificationsByUserIdResult> Handle(GetNotificationsByUserIdQuery query, CancellationToken cancellationToken)
    {
        if (!query.IsNotificationDepartment && query.UserId != query.CurrentUserId)
            throw new BuildingBlocks.Exceptions.ForbiddenAccessException("You are not authorized to view these notifications.");
        var notification = await notificationRepository.GetByUserIdAsync(query.UserId.ToString(), cancellationToken)
            ?? throw new NotificationNotFoundException();
        return new GetNotificationsByUserIdResult(notification);
    }
}