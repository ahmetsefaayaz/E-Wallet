using BuildingBlocks.Exceptions;

namespace Account.Application.Exceptions;

public class AccountNotFoundException: NotFoundException
{
    public AccountNotFoundException(string accountId) : base("Account",  accountId)
    {
    }
}