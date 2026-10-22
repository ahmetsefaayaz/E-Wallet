using Account.Domain.Exceptions;

namespace Account.Domain.ValueObjects;

public record CardNumber
{
    public string Value { get; init; }
    private CardNumber(string value) => Value = value;

    public static CardNumber Of(string value)
    {
        if(String.IsNullOrWhiteSpace(value))
            throw new DomainException("CardNumber cannot be empty");
        return new CardNumber(value);
    }
}