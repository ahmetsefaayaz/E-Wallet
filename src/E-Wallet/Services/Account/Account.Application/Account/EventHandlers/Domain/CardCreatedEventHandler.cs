using Account.Domain.Events;
using BuildingBlocks.Messaging.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Account.Application.Account.EventHandlers.Domain;

internal sealed class CardCreatedEventHandler(ILogger<CardCreatedEventHandler> logger, IPublishEndpoint publishEndpoint)
    : INotificationHandler<CardCreatedEvent>
{
    public async Task Handle(CardCreatedEvent notification, CancellationToken cancellationToken)
    {
        var cardIntegrationEvent = new CardCreatedIntegrationEvent
        {
            UserId = notification.UserId.Value,
            AccountId = notification.Card.AccountId.Value,
            CardId = notification.Card.Id.Value,
            CardHolderName = notification.Card.CardHolderName.Value,
            CardNumber = notification.Card.CardNumber.Value,
            CardExpiryDate = notification.Card.CardExpiryDate.Value,
            CardSecurityNumber = notification.Card.CardSecurityNumber.Value
        };
        logger.LogInformation("Card created {Card}", notification.Card);
        await publishEndpoint.Publish(cardIntegrationEvent, cancellationToken);
    }
}