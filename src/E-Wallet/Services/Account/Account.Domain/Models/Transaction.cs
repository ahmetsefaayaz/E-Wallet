using Account.Domain.Abstractions;
using Account.Domain.ValueObjects;

namespace Account.Domain.Models;

public class Transaction: Entity<TransactionId>
{
    internal Transaction(AccountId accountId, TransactionType transactionType, Status status, decimal amount)
    {
        Id = TransactionId.Of(Guid.NewGuid());
        AccountId = accountId;
        TransactionType = transactionType;
        Status = status;
        Amount = amount;
    }
    public AccountId AccountId { get; private set; }
    public TransactionType TransactionType { get; private set; }
    public Status Status { get; private set; }
    public decimal Amount { get; private set; }
}