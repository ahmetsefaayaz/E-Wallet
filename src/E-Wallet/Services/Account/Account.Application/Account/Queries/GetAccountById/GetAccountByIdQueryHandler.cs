using Account.Application.Exceptions;
using Account.Application.Extensions;
using Account.Application.Interfaces;
using Account.Domain.ValueObjects;
using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;

namespace Account.Application.Account.Queries.GetAccountById;

public class GetAccountByIdQueryHandler(IAccountDbContext dbContext)
    : IQueryHandler<GetAccountByIdQuery, GetAccountByIdResult>
{
    public async Task<GetAccountByIdResult> Handle(GetAccountByIdQuery query, CancellationToken cancellationToken)
    {
        var accountId = AccountId.Of(query.AccountId);
        var account = await dbContext.Accounts
            .AsNoTracking()
            .Include(a => a.Cards)
            .Include(a => a.Transactions)
            .FirstOrDefaultAsync(a => a.Id == accountId, cancellationToken)
                ?? throw new AccountNotFoundException(accountId.Value.ToString());
        return new GetAccountByIdResult(account.ToGetAccountDto());
    }
}