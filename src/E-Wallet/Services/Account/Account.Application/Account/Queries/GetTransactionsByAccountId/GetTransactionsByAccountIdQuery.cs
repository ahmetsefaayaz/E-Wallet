using Account.Application.Dtos;
using BuildingBlocks.CQRS;

namespace Account.Application.Account.Queries.GetTransactionsByAccountId;

public record GetTransactionsByAccountIdQuery(Guid AccountId): IQuery<GetTransactionsByAccountIdResult>;
public record GetTransactionsByAccountIdResult(IEnumerable<TransactionDto> Transactions);