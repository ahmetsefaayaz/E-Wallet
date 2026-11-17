using Account.Application.Account.EventHandlers.Domain;
using Account.Domain.Events;
using Account.Domain.ValueObjects;
using BuildingBlocks.Messaging.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;

namespace Account.UnitTests.Application.EventHandlers;

public class CardCreatedEventHandlerTests
{
    private readonly Mock<ILogger<CardCreatedEventHandler>> _loggerMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly CardCreatedEventHandler _handler;

    public CardCreatedEventHandlerTests()
    {
        _loggerMock = new Mock<ILogger<CardCreatedEventHandler>>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _handler = new CardCreatedEventHandler(_loggerMock.Object, _publishEndpointMock.Object);
    }

    [Fact]
    public async Task Handle_WhenCardCreatedEventReceived_ShouldPublishCardCreatedIntegrationEvent()
    {
        // Arrange
        var userId = UserId.Of(Guid.NewGuid());
        var account = new Account.Domain.Models.Account(userId, 1000m);
        var card = account.CreateCard(
            CardHolderName.Of("Ayaz"),
            CardNumber.Of("1234-5678-9012-3456"),
            CardExpiryDate.Of("12/28"),
            CardSecurityNumber.Of("123"));

        var domainEvent = new CardCreatedEvent(userId, card);

        // Act
        await _handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        _publishEndpointMock.Verify(
            x => x.Publish(
                It.Is<CardCreatedIntegrationEvent>(e =>
                    e.UserId == userId.Value &&
                    e.AccountId == account.Id.Value &&
                    e.CardId == card.Id.Value &&
                    e.CardHolderName == "Ayaz" &&
                    e.CardNumber == "1234-5678-9012-3456" &&
                    e.CardExpiryDate == "12/28" &&
                    e.CardSecurityNumber == "123"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
