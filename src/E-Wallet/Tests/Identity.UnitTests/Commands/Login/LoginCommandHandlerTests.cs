using BuildingBlocks.Exceptions;
using FluentAssertions;
using Identity.Api.Entities;
using Identity.Api.Exceptions;
using Identity.Api.Identity.Commands.Login;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Identity.UnitTests.Commands.Login;

public class LoginCommandHandlerTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IJwtCreator> _jwtCreatorMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        var userStoreMock = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(
            userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        _configurationMock = new Mock<IConfiguration>();
        _jwtCreatorMock = new Mock<IJwtCreator>();

        _handler = new LoginCommandHandler(
            _userManagerMock.Object,
            _configurationMock.Object,
            _jwtCreatorMock.Object);
    }

    [Fact]
    public async Task Handle_WhenCredentialsAreValid_ShouldReturnJwtToken()
    {
        // Arrange
        var fakeUser = new User { Id = Guid.NewGuid(), Email = "ayaz@test.com", UserName = "ayaz" };
        var command = new LoginCommand("ayaz@test.com", "Password123");

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(command.Email))
            .ReturnsAsync(fakeUser);

        _userManagerMock
            .Setup(x => x.CheckPasswordAsync(fakeUser, command.Password))
            .ReturnsAsync(true);

        _jwtCreatorMock
            .Setup(x => x.GenerateJwtToken(_userManagerMock.Object, fakeUser, _configurationMock.Object))
            .ReturnsAsync("fake-jwt-token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("fake-jwt-token");
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldThrowUserNotFoundException()
    {
        // Arrange
        var command = new LoginCommand("nonexistent@test.com", "Password123");

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(command.Email))
            .ReturnsAsync((User?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UserNotFoundException>();
        _jwtCreatorMock.Verify(
            x => x.GenerateJwtToken(It.IsAny<UserManager<User>>(), It.IsAny<User>(), It.IsAny<IConfiguration>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenPasswordIsIncorrect_ShouldThrowBadRequestException()
    {
        // Arrange
        var fakeUser = new User { Id = Guid.NewGuid(), Email = "ayaz@test.com", UserName = "ayaz" };
        var command = new LoginCommand("ayaz@test.com", "WrongPassword123");

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(command.Email))
            .ReturnsAsync(fakeUser);

        _userManagerMock
            .Setup(x => x.CheckPasswordAsync(fakeUser, command.Password))
            .ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("wrong password");
        _jwtCreatorMock.Verify(
            x => x.GenerateJwtToken(It.IsAny<UserManager<User>>(), It.IsAny<User>(), It.IsAny<IConfiguration>()),
            Times.Never);
    }
}
