using Account.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Account.Application.Account.EventHandlers.Domain;

public class CardCreatedEventHandler(ILogger<CardCreatedEventHandler> logger)
    : INotificationHandler<CardCreatedEvent>
{
    public Task Handle(CardCreatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Card created {Card}", notification.Card);
        return Task.CompletedTask;
    }
}