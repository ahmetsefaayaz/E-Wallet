using Account.Domain.Exceptions;

namespace Account.Domain.ValueObjects;

public record CardId
{
    public Guid Value { get; init; }
    private CardId(Guid value) => Value = value;

    public static CardId Of(Guid value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value == Guid.Empty) throw new DomainException("Card Id cannot be empty");
        return new CardId(value);
    }
}