using Account.Domain.Exceptions;

namespace Account.Domain.ValueObjects;

public record CardSecurityNumber
{
    public string Value { get; init; }
    private CardSecurityNumber(string value) => Value = value;

    public static CardSecurityNumber Of(string value)
    {
        if(String.IsNullOrWhiteSpace(value))
            throw new DomainException("CardSecurityNumber cannot be empty");
        return new CardSecurityNumber(value);
    }
}