using Account.Domain.Exceptions;

namespace Account.Domain.ValueObjects;

public record IdempotencyId
{
    public Guid Value { get; init; }
    private IdempotencyId(Guid value) => Value = value;
    public static IdempotencyId Of(Guid value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value == Guid.Empty) throw new DomainException("Idempotency Id cannot be empty");
        return new IdempotencyId(value);
    }
}