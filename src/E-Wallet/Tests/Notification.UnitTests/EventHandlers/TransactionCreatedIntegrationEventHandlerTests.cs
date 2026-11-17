using BuildingBlocks.Messaging.Events;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Notification.Api.Entities;
using Notification.Api.Exceptions;
using Notification.Api.Interfaces;
using Notification.Api.Notification.EventHandlers;

namespace Notification.UnitTests.EventHandlers;

public class TransactionCreatedIntegrationEventHandlerTests
{
    private readonly Mock<ILogger<TransactionCreatedIntegrationEventHandler>> _loggerMock;
    private readonly Mock<INotificationRepository> _notificationRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly TransactionCreatedIntegrationEventHandler _handler;

    public TransactionCreatedIntegrationEventHandlerTests()
    {
        _loggerMock = new Mock<ILogger<TransactionCreatedIntegrationEventHandler>>();
        _notificationRepositoryMock = new Mock<INotificationRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _emailServiceMock = new Mock<IEmailService>();

        _handler = new TransactionCreatedIntegrationEventHandler(
            _loggerMock.Object,
            _notificationRepositoryMock.Object,
            _userRepositoryMock.Object,
            _emailServiceMock.Object);
    }

    [Fact]
    public async Task Consume_WhenUserExists_ShouldSendEmailAndSaveNotification()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fakeUser = new User { UserId = userId, Email = "ayaz@test.com", Username = "ayaz" };
        var integrationEvent = new TransactionCreatedIntegrationEvent
        {
            UserId = userId,
            AccountId = Guid.NewGuid(),
            TransactionId = Guid.NewGuid(),
            CardId = Guid.NewGuid(),
            Amount = 750m,
            TransactionType = "Deposit"
        };

        var consumeContextMock = new Mock<ConsumeContext<TransactionCreatedIntegrationEvent>>();
        consumeContextMock.Setup(x => x.Message).Returns(integrationEvent);
        consumeContextMock.Setup(x => x.CancellationToken).Returns(CancellationToken.None);

        _userRepositoryMock
            .Setup(x => x.GetAsync(userId, CancellationToken.None))
            .ReturnsAsync(fakeUser);

        // Act
        await _handler.Consume(consumeContextMock.Object);

        // Assert
        _emailServiceMock.Verify(
            x => x.SendEmailAsync(
                "ayaz@test.com",
                "Transaction Notification",
                It.Is<string>(body => body.Contains("Deposit") && body.Contains("750"))),
            Times.Once);

        _notificationRepositoryMock.Verify(
            x => x.AddAsync(
                It.Is<Notification.Api.Entities.Notification>(n =>
                    n.UserId == userId &&
                    n.AccountId == integrationEvent.AccountId &&
                    n.Type == "Deposit"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Consume_WhenUserDoesNotExist_ShouldThrowUserNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var integrationEvent = new TransactionCreatedIntegrationEvent
        {
            UserId = userId,
            AccountId = Guid.NewGuid(),
            TransactionId = Guid.NewGuid(),
            Amount = 500m,
            TransactionType = "Withdrawal"
        };

        var consumeContextMock = new Mock<ConsumeContext<TransactionCreatedIntegrationEvent>>();
        consumeContextMock.Setup(x => x.Message).Returns(integrationEvent);

        _userRepositoryMock
            .Setup(x => x.GetAsync(userId, CancellationToken.None))
            .ReturnsAsync((User?)null);

        // Act
        Func<Task> act = async () => await _handler.Consume(consumeContextMock.Object);

        // Assert
        await act.Should().ThrowAsync<UserNotFoundException>();
        _emailServiceMock.Verify(
            x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
        _notificationRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Notification.Api.Entities.Notification>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
