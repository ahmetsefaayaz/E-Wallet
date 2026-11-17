using BuildingBlocks.Exceptions;
using FluentAssertions;
using Identity.Api.Data;
using Identity.Api.Entities;
using Identity.Api.Identity.Commands.Register;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Identity.UnitTests.Commands.Register;

public class RegisterCommandHandlerTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly IdentityServiceDbContext _dbContext;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        var userStoreMock = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(
            userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        _publishEndpointMock = new Mock<IPublishEndpoint>();

        var options = new DbContextOptionsBuilder<IdentityServiceDbContext>()
            .Options;
        _dbContext = new Mock<IdentityServiceDbContext>(options).Object;

        _handler = new RegisterCommandHandler(
            _userManagerMock.Object,
            _publishEndpointMock.Object,
            _dbContext);
    }

    [Fact]
    public async Task Handle_WhenEmailAlreadyExists_ShouldThrowBadRequestException()
    {
        // Arrange
        var existingUser = new User { Id = Guid.NewGuid(), Email = "existing@test.com", UserName = "existing" };
        var command = new RegisterCommand("ayaz", "existing@test.com", "Password123", "Password123");

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(command.Email))
            .ReturnsAsync(existingUser);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Email already exists");
        _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenPasswordsDoNotMatch_ShouldThrowBadRequestException()
    {
        // Arrange
        var command = new RegisterCommand("ayaz", "new@test.com", "Password123", "DifferentPassword123");

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(command.Email))
            .ReturnsAsync((User?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Passwords do not match");
        _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
    }
}
