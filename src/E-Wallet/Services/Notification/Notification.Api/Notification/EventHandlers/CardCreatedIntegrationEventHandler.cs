using BuildingBlocks.Messaging.Events;
using MassTransit;
using Notification.Api.Entities.Enums;
using Notification.Api.Exceptions;
using Notification.Api.Interfaces;

namespace Notification.Api.Notification.EventHandlers;

public class CardCreatedIntegrationEventHandler(INotificationRepository notificationRepository, 
    ILogger<CardCreatedIntegrationEventHandler> logger, IUserRepository userRepository, IEmailService emailService)
    : IConsumer<CardCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<CardCreatedIntegrationEvent> context)
    {
        var @event = context.Message;
        var userMail = await GetUserMail(@event.UserId);
        await SendMail(userMail, @event);
        
        
        var notification = await CreateNotification(@event, context.CancellationToken);
        
    }

    private async Task<Entities.Notification> CreateNotification(CardCreatedIntegrationEvent @event, CancellationToken cancellationToken)
    {
        var title = "Card Created";
        var message = $"Card {@event.CardId}  has been created";
        var type = "CardCreated";
        var notification = new Entities.Notification(
            @event.UserId,
            @event.AccountId,
            title,
            message,
            type,
            NotificationChannel.Email,
            @event.CardId,
            DateTime.UtcNow
        );
        await notificationRepository.AddAsync(notification, cancellationToken);
        return notification;
    }
    private async Task SendMail(string toMail, CardCreatedIntegrationEvent @event)
    {
        string mailBody = $"New Card({@event.CardNumber}) created.";
        await emailService.SendEmailAsync(toMail, "Card Notification", mailBody);
    }

    private async Task<string> GetUserMail(Guid userId)
    {
        var user = await userRepository.GetAsync(userId)
                   ?? throw new UserNotFoundException(userId);
        return user.Email;
    }
}