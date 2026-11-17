using FluentAssertions;
using Moq;
using Notification.Api.Entities.Enums;
using Notification.Api.Exceptions;
using Notification.Api.Interfaces;
using Notification.Api.Notification.Commands;

namespace Notification.UnitTests.Commands;

public class MarkAsReadCommandHandlerTests
{
    private readonly Mock<INotificationRepository> _notificationRepositoryMock;
    private readonly MarkAsReadCommandHandler _handler;

    public MarkAsReadCommandHandlerTests()
    {
        _notificationRepositoryMock = new Mock<INotificationRepository>();
        _handler = new MarkAsReadCommandHandler(_notificationRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenNotificationExists_ShouldMarkAsReadAndReturnSuccess()
    {
        // Arrange
        var notificationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var fakeNotification = new Notification.Api.Entities.Notification(
            userId: userId,
            accountId: Guid.NewGuid(),
            title: "Transaction Created",
            message: "Test message",
            type: "Deposit",
            channel: NotificationChannel.Email,
            referenceId: null,
            createdAt: DateTime.UtcNow);

        _notificationRepositoryMock
            .Setup(x => x.GetByIdAsync(notificationId.ToString(), userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeNotification);

        var command = new MarkAsReadCommand(notificationId, userId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _notificationRepositoryMock.Verify(
            x => x.MarkAsReadAsync(fakeNotification, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNotificationDoesNotExist_ShouldThrowNotificationNotFoundException()
    {
        // Arrange
        var notificationId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _notificationRepositoryMock
            .Setup(x => x.GetByIdAsync(notificationId.ToString(), userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Notification.Api.Entities.Notification?)null);

        var command = new MarkAsReadCommand(notificationId, userId);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotificationNotFoundException>();
        _notificationRepositoryMock.Verify(
            x => x.MarkAsReadAsync(It.IsAny<Notification.Api.Entities.Notification>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
