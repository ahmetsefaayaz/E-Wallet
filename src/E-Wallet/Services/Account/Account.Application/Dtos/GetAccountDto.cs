using Account.Domain.ValueObjects;

namespace Account.Application.Dtos;

public record GetAccountDto(
    Guid Id,
    decimal AccountBalance,
    IEnumerable<GetCardDto> Cards,
    IEnumerable<TransactionDto> Transactions);