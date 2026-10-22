using Account.Domain.Exceptions;

namespace Account.Domain.ValueObjects;

public record CardExpiryDate
{
    public string Value { get; init; }
    private CardExpiryDate(string value) => Value = value;

    public static CardExpiryDate Of(string value)
    {
        if(String.IsNullOrWhiteSpace(value))
            throw new DomainException("CardExpiryDate cannot be empty");
        return new CardExpiryDate(value);
    }
}