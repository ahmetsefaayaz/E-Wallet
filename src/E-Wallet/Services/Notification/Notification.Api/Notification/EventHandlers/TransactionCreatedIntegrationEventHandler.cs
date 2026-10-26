using BuildingBlocks.Messaging.Events;
using MassTransit;
using Notification.Api.Entities.Enums;
using Notification.Api.Exceptions;
using Notification.Api.Interfaces;

namespace Notification.Api.Notification.EventHandlers;

public class TransactionCreatedIntegrationEventHandler(ILogger<TransactionCreatedIntegrationEventHandler> logger,
    INotificationRepository notificationRepository, IUserRepository userRepository, IEmailService emailService)
    :IConsumer<TransactionCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<TransactionCreatedIntegrationEvent> context)
    {
        var @event = context.Message;
        var notification = await CreateNotification(@event, context.CancellationToken);
        
        var userMail = await GetUserMail(@event.UserId);
        await SendMail(userMail, @event);

    }

    private async Task<Entities.Notification> CreateNotification(TransactionCreatedIntegrationEvent @event, 
        CancellationToken cancellationToken)
    {
        var title = "Transaction Created";
        var message = $"Transaction {@event.TransactionId}  has been created";
        var transactionType = @event.TransactionType;
        
        var notification = new Entities.Notification(
            @event.UserId,
            @event.AccountId,
            title,
            message,
            transactionType,
            NotificationChannel.Email,
            @event.CardId,
            DateTime.UtcNow);
        await notificationRepository.AddAsync(notification, cancellationToken);
        return notification;
    }

    private async Task SendMail(string toMail, TransactionCreatedIntegrationEvent @event)
    {
        string mailBody = $"New transaction {@event.TransactionType} occured with {@event.Amount} amount.";
        await emailService.SendEmailAsync(toMail, "Transaction Notification", mailBody);
        
    }

    private async Task<string> GetUserMail(Guid userId)
    {
        var user = await userRepository.GetAsync(userId)
            ?? throw new UserNotFoundException(userId);
        return user.Email;
    }
}