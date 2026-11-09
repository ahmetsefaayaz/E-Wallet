using Account.Application.Dtos;
using BuildingBlocks.CQRS;

namespace Account.Application.Account.Queries.GetMyAccounts;

public record GetMyAccountsQuery(Guid UserId)
    : IQuery<GetMyAccountsResult>;

public record GetMyAccountsResult(IEnumerable<GetAccountDto> Accounts);