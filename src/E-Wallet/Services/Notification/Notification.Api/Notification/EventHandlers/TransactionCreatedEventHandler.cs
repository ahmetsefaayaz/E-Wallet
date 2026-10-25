using BuildingBlocks.Messaging.Events;
using MassTransit;
using Notification.Api.Entities.Enums;
using Notification.Api.Interfaces;

namespace Notification.Api.Notification.EventHandlers;

public class TransactionCreatedEventHandler(ILogger<TransactionCreatedEventHandler> logger,
    INotificationRepository repository)
    :IConsumer<TransactionCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<TransactionCreatedIntegrationEvent> context)
    {
        var @event = context.Message;
        var title = "Transaction Created";
        var message = $"Transaction {@event.TransactionId}  has been created";
        
        var notification = new Entities.Notification(
            @event.AccountId,
            title,
            message,
            NotificationType.CardCreated,
            NotificationChannel.Email,
            @event.CardId,
            DateTime.UtcNow);
        await repository.AddAsync(notification, context.CancellationToken);
        logger.LogInformation("Notification Created {NotificationId}", notification.Id);
    }
}