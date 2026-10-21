namespace Account.Domain.ValueObjects;

public enum TransactionType
{
    Withdrawal = 1,
    Deposit = 2,
    IncomingTransfer = 3,
    OutgoingTransfer = 4,
}