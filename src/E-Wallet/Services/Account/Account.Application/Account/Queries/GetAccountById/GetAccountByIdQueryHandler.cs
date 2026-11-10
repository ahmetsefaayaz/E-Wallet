using Account.Application.Exceptions;
using Account.Application.Extensions;
using Account.Application.Interfaces;
using Account.Domain.ValueObjects;
using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
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
        if (!query.IsAccountDepartment && account.UserId.Value != query.CurrentUserId)
            throw new ForbiddenAccessException("You are not authorized to view this account.");
        return new GetAccountByIdResult(account.ToGetAccountDto());
    }
}