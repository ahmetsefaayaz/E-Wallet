using Account.Application.Dtos;
using BuildingBlocks.CQRS;

namespace Account.Application.Account.Queries.GetCardsByAccountId;

public record GetCardsByAccountIdQuery(Guid AccountId): IQuery<GetCardsByAccountIdResult>;
public record GetCardsByAccountIdResult(IEnumerable<GetCardDto> Cards);