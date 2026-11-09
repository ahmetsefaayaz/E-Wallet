using Account.Application.Extensions;
using Account.Application.Interfaces;
using Account.Domain.ValueObjects;
using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;

namespace Account.Application.Account.Queries.GetMyAccounts;

public sealed class GetMyAccountsQueryHandler(IAccountDbContext dbContext)
    : IQueryHandler<GetMyAccountsQuery, GetMyAccountsResult>
{
    public async Task<GetMyAccountsResult> Handle(
        GetMyAccountsQuery query,
        CancellationToken cancellationToken)
    {
        var userId = UserId.Of(query.UserId);
        var accounts = await dbContext.Accounts
            .AsNoTracking()
            .Include(account => account.Cards)
            .Include(account => account.Transactions)
            .Where(account => account.UserId == userId)
            .ToListAsync(cancellationToken);
        var accountDtos = accounts.Select(a => a.ToGetAccountDto());

        return new GetMyAccountsResult(accountDtos);
    }
}