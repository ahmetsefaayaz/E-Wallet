using Account.Domain.Exceptions;

namespace Account.Domain.ValueObjects;

public record AccountId
{
    public Guid Value { get; init; }
    private AccountId(Guid value) => Value = value;

    public static AccountId Of(Guid value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value == Guid.Empty) throw new DomainException("AccountId Cannot Be Empty");
        return new AccountId(value);
    }
    
}