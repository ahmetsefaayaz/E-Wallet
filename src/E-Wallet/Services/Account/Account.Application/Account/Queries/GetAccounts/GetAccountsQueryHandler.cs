using Account.Application.Dtos;
using Account.Application.Extensions;
using Account.Application.Interfaces;
using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Account.Application.Account.Queries.GetAccounts;

public class GetAccountsQueryHandler(IAccountDbContext dbContext)
    : IQueryHandler<GetAccountsQuery, GetAccountsResult>
{
    public async Task<GetAccountsResult> Handle(GetAccountsQuery query, CancellationToken cancellationToken)
    {
        var accounts = await dbContext
            .Accounts.AsNoTracking()
            .OrderByDescending(a => a.CreatedAt)
            .Include (a => a.Cards)
            .Include (a => a.Transactions)
            .Skip(query.PaginationRequest.PageSize * query.PaginationRequest.PageIndex)
            .Take(query.PaginationRequest.PageIndex)
            .ToListAsync(cancellationToken);
        return new GetAccountsResult(new PaginatedResult<GetAccountDto>
            (query.PaginationRequest.PageIndex, query.PaginationRequest.PageSize,
                accounts.Count, accounts.ToGetAccountsDto()));
        
    }
}