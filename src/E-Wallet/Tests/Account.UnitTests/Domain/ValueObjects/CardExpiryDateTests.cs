using Account.Domain.Exceptions;
using Account.Domain.ValueObjects;
using FluentAssertions;
namespace Account.UnitTests.Domain.ValueObjects;
public sealed class CardExpiryDateTests
{
    [Fact]
    public void Of_WhenValueIsValid_ShouldReturnCardExpiryDate()
    {
        const string validExpiryDate = "12/28";
        var result = CardExpiryDate.Of(validExpiryDate);
        result.Should().NotBeNull();
        result.Value.Should().Be(validExpiryDate);
    }
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Of_WhenValueIsNullOrWhiteSpace_ShouldThrowDomainException(string? invalidValue)
    {
        var act = () => CardExpiryDate.Of(invalidValue!);
        act.Should()
            .Throw<DomainException>();
    }
    [Fact]
    public void Equality_WhenTwoInstancesHaveSameValue_ShouldBeEqual()
    {
        var expiry1 = CardExpiryDate.Of("08/29");
        var expiry2 = CardExpiryDate.Of("08/29");
        expiry1.Should().Be(expiry2);
    }
    [Fact]
    public void Equality_WhenTwoInstancesHaveDifferentValues_ShouldNotBeEqual()
    {
        var expiry1 = CardExpiryDate.Of("08/29");
        var expiry2 = CardExpiryDate.Of("09/29");
        expiry1.Should().NotBe(expiry2);
    }
}