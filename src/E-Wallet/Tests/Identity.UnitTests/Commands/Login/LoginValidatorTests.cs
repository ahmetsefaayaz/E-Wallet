using FluentAssertions;
using Identity.Api.Identity.Commands.Login;

namespace Identity.UnitTests.Commands.Login;

public class LoginValidatorTests
{
    private readonly LoginValidator _validator = new();

    [Fact]
    public void Validate_WhenEmailAndPasswordAreValid_ShouldBeValid()
    {
        // Arrange
        var command = new LoginCommand("ayaz@test.com", "Password123");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WhenEmailIsEmpty_ShouldHaveValidationError(string invalidEmail)
    {
        // Arrange
        var command = new LoginCommand(invalidEmail, "Password123");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(LoginCommand.Email));
    }

    [Fact]
    public void Validate_WhenEmailExceedsMaxLength_ShouldHaveValidationError()
    {
        // Arrange
        var longEmail = new string('a', 115) + "@test.com"; // > 120 chars
        var command = new LoginCommand(longEmail, "Password123");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(LoginCommand.Email));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WhenPasswordIsEmpty_ShouldHaveValidationError(string invalidPassword)
    {
        // Arrange
        var command = new LoginCommand("ayaz@test.com", invalidPassword);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(LoginCommand.Password));
    }
}
