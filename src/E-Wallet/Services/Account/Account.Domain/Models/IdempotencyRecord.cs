using Account.Domain.Abstractions;
using Account.Domain.ValueObjects;

namespace Account.Domain.Models;

public sealed class IdempotencyRecord: Entity<IdempotencyId>
{
    public UserId UserId { get; private set; }
    public string Operation { get; private set; }
    public string Key { get; private set; }
    public IdempotencyRecord(UserId userId, string operation, string key)
    {
        Id = IdempotencyId.Of(Guid.NewGuid());
        UserId = userId;
        Operation = operation;
        Key = key;
    }
    
}