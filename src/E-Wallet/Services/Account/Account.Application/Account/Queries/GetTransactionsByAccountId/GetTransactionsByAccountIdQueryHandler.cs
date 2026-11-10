using Account.Application.Exceptions;
using Account.Application.Extensions;
using Account.Application.Interfaces;
using Account.Domain.ValueObjects;
using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Account.Application.Account.Queries.GetTransactionsByAccountId;

public class GetTransactionsByAccountIdQueryHandler(IAccountDbContext dbContext)
    : IQueryHandler<GetTransactionsByAccountIdQuery, GetTransactionsByAccountIdResult>
{
    public async Task<GetTransactionsByAccountIdResult> Handle(GetTransactionsByAccountIdQuery query,
        CancellationToken cancellationToken)
    {
        var accountId = AccountId.Of(query.AccountId);
        var account = await dbContext.Accounts
            .AsNoTracking()
            .Include(a => a.Transactions)
            .FirstOrDefaultAsync(a => a.Id == accountId, cancellationToken)
                ?? throw new AccountNotFoundException(accountId.Value.ToString());
        if (!query.IsAccountDepartment && account.UserId.Value != query.CurrentUserId)
            throw new ForbiddenAccessException("You are not authorized to view these transactions.");
        return new GetTransactionsByAccountIdResult(account.Transactions.ToTransactionsDto());
    }
}