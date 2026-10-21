using Account.Domain.Exceptions;

namespace Account.Domain.ValueObjects;

public record TransactionId
{
    public Guid Value { get; init; }
    private  TransactionId(Guid value) => Value = value;

    public static TransactionId Of(Guid value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value == Guid.Empty) throw new DomainException("TransactionId Cannot Be Empty");
        return new TransactionId(value);
    }
    
}