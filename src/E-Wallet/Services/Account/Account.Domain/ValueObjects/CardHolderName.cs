using Account.Domain.Exceptions;

namespace Account.Domain.ValueObjects;

public record CardHolderName
{
    public string Value { get; init; }
    private CardHolderName(string value) => Value = value;

    public static CardHolderName Of(string value)
    {
        if(String.IsNullOrWhiteSpace(value))
            throw new DomainException("CardHolderName cannot be empty");
        return new CardHolderName(value);
    }
}