using Account.Application.Dtos;
using Account.Application.Extensions;
using Account.Application.Interfaces;
using Account.Domain.ValueObjects;
using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Account.Application.Account.Queries.GetAccounts;

public class GetAccountsQueryHandler(IAccountDbContext dbContext)
    : IQueryHandler<GetAccountsQuery, GetAccountsResult>
{
    public async Task<GetAccountsResult> Handle(GetAccountsQuery query, CancellationToken cancellationToken)
    {
        var currentUserId = UserId.Of(query.CurrentUserId);
        var totalCount = await dbContext.Accounts.LongCountAsync(cancellationToken);
        var accounts = await dbContext
            .Accounts.AsNoTracking()
            .Where(a => query.IsAccountDepartment || a.UserId == currentUserId)
            .OrderByDescending(a => a.CreatedAt)
            .Include (a => a.Cards)
            .Include (a => a.Transactions)
            .Skip(query.PaginationRequest.PageSize * query.PaginationRequest.PageIndex)
            .Take(query.PaginationRequest.PageSize)
            .ToListAsync(cancellationToken);
        return new GetAccountsResult(new PaginatedResult<GetAccountDto>
            (query.PaginationRequest.PageIndex, query.PaginationRequest.PageSize,
                totalCount, accounts.ToGetAccountsDto()));
        
    }
}