using Account.Domain.ValueObjects;

namespace Account.Application.Dtos;

public record GetAccountDto(
    Guid Id,
    decimal AccountBalance,
    List<GetCardDto> Cards,
    List<TransactionDto> Transactions);