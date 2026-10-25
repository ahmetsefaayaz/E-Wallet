using BuildingBlocks.Messaging.Events;
using MassTransit;
using Notification.Api.Entities.Enums;
using Notification.Api.Interfaces;

namespace Notification.Api.Notification.EventHandlers;

public class CardCreatedIntegrationEventHandler(INotificationRepository notificationRepository, 
    ILogger<CardCreatedIntegrationEventHandler> logger)
    : IConsumer<CardCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<CardCreatedIntegrationEvent> context)
    {
        var @event = context.Message;
        var title = "Card Created";
        var message = $"Card {@event.CardId}  has been created";
        var notification = new Entities.Notification(
            @event.AccountId,
            title,
            message,
            NotificationType.CardCreated,
            NotificationChannel.Email,
            @event.CardId,
            DateTime.UtcNow
        );
        await notificationRepository.AddAsync(notification, context.CancellationToken);
        //temporary log, this will be email later
        logger.LogInformation("Notification Created {NotificationId}", notification.Id);
    }
}