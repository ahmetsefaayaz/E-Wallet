using Account.Application.Account.Commands.TransferMoney;
using FluentAssertions;

namespace Account.UnitTests.Application.Commands.TransferMoney;

public class OutgoingTransactionValidatorTests
{
    private readonly OutgoingTransactionValidator _validator = new();

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldBeValid()
    {
        // Arrange
        var command = new TransferMoneyCommand(
            AccountId: Guid.NewGuid(),
            ReceiverAccountId: Guid.NewGuid(),
            CardId: Guid.NewGuid(),
            CurrentUserId: Guid.NewGuid(),
            Amount: 250m,
            IdempotencyKey: "valid-idempotency-key");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    [InlineData(1_000_000)]
    [InlineData(1_500_000)]
    public void Validate_WhenAmountIsOutOfBounds_ShouldHaveValidationError(decimal invalidAmount)
    {
        // Arrange
        var command = new TransferMoneyCommand(
            AccountId: Guid.NewGuid(),
            ReceiverAccountId: Guid.NewGuid(),
            CardId: Guid.NewGuid(),
            CurrentUserId: Guid.NewGuid(),
            Amount: invalidAmount,
            IdempotencyKey: "valid-idempotency-key");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(TransferMoneyCommand.Amount));
    }

    [Theory]
    [InlineData("")]
    [InlineData("1234")]
    public void Validate_WhenIdempotencyKeyIsInvalid_ShouldHaveValidationError(string invalidKey)
    {
        // Arrange
        var command = new TransferMoneyCommand(
            AccountId: Guid.NewGuid(),
            ReceiverAccountId: Guid.NewGuid(),
            CardId: Guid.NewGuid(),
            CurrentUserId: Guid.NewGuid(),
            Amount: 100m,
            IdempotencyKey: invalidKey);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(TransferMoneyCommand.IdempotencyKey));
    }

    [Fact]
    public void Validate_WhenCardIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new TransferMoneyCommand(
            AccountId: Guid.NewGuid(),
            ReceiverAccountId: Guid.NewGuid(),
            CardId: Guid.Empty,
            CurrentUserId: Guid.NewGuid(),
            Amount: 100m,
            IdempotencyKey: "valid-idempotency-key");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(TransferMoneyCommand.CardId));
    }
}
