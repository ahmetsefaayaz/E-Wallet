using Account.Domain.ValueObjects;

namespace Account.Application.Dtos;

public record GetAccountDto(
    Guid Id,
    Guid UserId,
    decimal AccountBalance,
    IEnumerable<GetCardDto> Cards,
    IEnumerable<TransactionDto> Transactions);