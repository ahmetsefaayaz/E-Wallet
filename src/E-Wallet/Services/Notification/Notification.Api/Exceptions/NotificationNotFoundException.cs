using BuildingBlocks.Exceptions;

namespace Notification.Api.Exceptions;

public class NotificationNotFoundException: NotFoundException
{
    public NotificationNotFoundException(string notificationId) : base("Notification",  notificationId)
    {
    }
}