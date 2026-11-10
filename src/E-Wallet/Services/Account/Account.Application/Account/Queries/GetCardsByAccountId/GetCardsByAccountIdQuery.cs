using Account.Application.Dtos;
using BuildingBlocks.CQRS;

namespace Account.Application.Account.Queries.GetCardsByAccountId;

public record GetCardsByAccountIdQuery(Guid AccountId, Guid CurrentUserId, bool IsAccountDepartment): IQuery<GetCardsByAccountIdResult>;
public record GetCardsByAccountIdResult(IEnumerable<GetCardDto> Cards);