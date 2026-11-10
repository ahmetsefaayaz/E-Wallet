using Account.Application.Dtos;
using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;

namespace Account.Application.Account.Queries.GetAccounts;

public record GetAccountsResult(PaginatedResult<GetAccountDto> PaginatedResult);

public record GetAccountsQuery(PaginationRequest PaginationRequest, Guid CurrentUserId, bool IsAccountDepartment): IQuery<GetAccountsResult>;