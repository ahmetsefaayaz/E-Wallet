using BuildingBlocks.CQRS;
using Notification.Api.Exceptions;
using Notification.Api.Interfaces;

namespace Notification.Api.Notification.Commands;

public record MarkAsReadCommand(Guid NotificationId, Guid CurrentUserId) : ICommand<MarkAsReadResult>;
public record MarkAsReadResult(bool IsSuccess);
internal sealed class MarkAsReadCommandHandler(INotificationRepository notificationRepository)
    : ICommandHandler<MarkAsReadCommand, MarkAsReadResult>
{
    public async Task<MarkAsReadResult> Handle(MarkAsReadCommand command, CancellationToken cancellationToken)
    {
        var notification = await notificationRepository.GetByIdAsync(
                               command.NotificationId.ToString(),
                               command.CurrentUserId.ToString(),
                               cancellationToken)
                           ?? throw new NotificationNotFoundException(command.NotificationId.ToString());
        await notificationRepository.MarkAsReadAsync(notification, cancellationToken);
        return new MarkAsReadResult(true);
    }
}