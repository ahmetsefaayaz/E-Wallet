using Account.Application.Account.EventHandlers.Domain;
using Account.Domain.Events;
using Account.Domain.ValueObjects;
using BuildingBlocks.Messaging.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;

namespace Account.UnitTests.Application.EventHandlers;

public class TransactionCreatedEventHandlerTests
{
    private readonly Mock<ILogger<TransactionCreatedEventHandler>> _loggerMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly TransactionCreatedEventHandler _handler;

    public TransactionCreatedEventHandlerTests()
    {
        _loggerMock = new Mock<ILogger<TransactionCreatedEventHandler>>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _handler = new TransactionCreatedEventHandler(_loggerMock.Object, _publishEndpointMock.Object);
    }

    [Fact]
    public async Task Handle_WhenTransactionCreatedEventReceived_ShouldPublishTransactionCreatedIntegrationEvent()
    {
        // Arrange
        var userId = UserId.Of(Guid.NewGuid());
        var account = new Account.Domain.Models.Account(userId, 1000m);
        var cardId = CardId.Of(Guid.NewGuid());
        var transaction = account.CreateTransaction(cardId, TransactionType.Deposit, 500m);

        var domainEvent = new TransactionCreatedEvent(userId, transaction);

        // Act
        await _handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        _publishEndpointMock.Verify(
            x => x.Publish(
                It.Is<TransactionCreatedIntegrationEvent>(e =>
                    e.UserId == userId.Value &&
                    e.AccountId == account.Id.Value &&
                    e.TransactionId == transaction.Id.Value &&
                    e.CardId == cardId.Value &&
                    e.Amount == 500m &&
                    e.TransactionType == TransactionType.Deposit.ToString()),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
