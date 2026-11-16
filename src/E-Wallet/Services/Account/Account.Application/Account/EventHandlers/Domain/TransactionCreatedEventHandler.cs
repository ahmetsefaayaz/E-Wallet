using Account.Domain.Events;
using BuildingBlocks.Messaging.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Account.Application.Account.EventHandlers.Domain;

internal sealed class TransactionCreatedEventHandler(ILogger<TransactionCreatedEventHandler>  logger,
    IPublishEndpoint publishEndpoint)
    : INotificationHandler<TransactionCreatedEvent>
{
    public async Task Handle(TransactionCreatedEvent notification, CancellationToken cancellationToken)
    {
        var transactionIntegrationEvent = new TransactionCreatedIntegrationEvent
        {
            UserId = notification.UserId.Value,
            AccountId = notification.Transaction.AccountId.Value,
            TransactionId = notification.Transaction.Id.Value,
            CardId = notification.Transaction.CardId?.Value,
            Amount = notification.Transaction.Amount,
            TransactionType = notification.Transaction.TransactionType.ToString()
        };
        logger.LogInformation("Transaction Created: {Transaction}",  notification.Transaction);
        
        await publishEndpoint.Publish(transactionIntegrationEvent, cancellationToken);
    }
}