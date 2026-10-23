using BuildingBlocks.Exceptions;

namespace Account.Application.Exceptions;

public class TransactionNotFoundException: NotFoundException
{
    public TransactionNotFoundException(string transactionId) : base("Transaction", transactionId)
    {
    }
}