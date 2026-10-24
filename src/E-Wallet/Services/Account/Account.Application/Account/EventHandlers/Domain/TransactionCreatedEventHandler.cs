using Account.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Account.Application.Account.EventHandlers.Domain;

public class TransactionCreatedEventHandler(ILogger<TransactionCreatedEventHandler>  logger)
    : INotificationHandler<TransactionCreatedEvent>
{
    public Task Handle(TransactionCreatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Transaction Created: {Transaction}",  notification.Transaction);
        return Task.CompletedTask;
    }
}