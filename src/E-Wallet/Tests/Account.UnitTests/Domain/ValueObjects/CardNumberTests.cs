using Account.Domain.Exceptions;
using Account.Domain.ValueObjects;
using FluentAssertions;

namespace Account.UnitTests.Domain.ValueObjects;

public class CardNumberTests
{
    [Fact]
    public void Of_GivenValidCardNumber_ShouldReturnCardNumberInstance()
    {
        // Arrange
        var validCardNumber = "1234567890123456";

        // Act
        var cardNumber = CardNumber.Of(validCardNumber);

        // Assert
        cardNumber.Should().NotBeNull();
        cardNumber.Value.Should().Be(validCardNumber);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Of_GivenNullOrWhiteSpace_ShouldThrowDomainException(string? invalidValue)
    {
        var act = () => CardNumber.Of(invalidValue!);

        act.Should()
            .Throw<DomainException>();
    }
    [Fact]
    public void Equality_WhenTwoInstancesHaveSameValue_ShouldBeEqual()
    {
        var cardNumber1 = CardNumber.Of("1234567890123456");
        var cardNumber2 = CardNumber.Of("1234567890123456");

        cardNumber1.Should().Be(cardNumber2);
    }
    [Fact]
    public void Equality_WhenTwoInstancesHaveDifferentValues_ShouldNotBeEqual()
    {
        var cardNumber1 = CardNumber.Of("1234567890123456");
        var cardNumber2 = CardNumber.Of("1234567890123457");

        cardNumber1.Should().NotBe(cardNumber2);
    }
}