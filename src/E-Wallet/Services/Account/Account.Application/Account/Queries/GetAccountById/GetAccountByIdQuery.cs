using Account.Application.Dtos;
using BuildingBlocks.CQRS;

namespace Account.Application.Account.Queries.GetAccountById;

public record GetAccountByIdQuery(Guid AccountId): IQuery<GetAccountByIdResult>;
public record GetAccountByIdResult(GetAccountDto Account);