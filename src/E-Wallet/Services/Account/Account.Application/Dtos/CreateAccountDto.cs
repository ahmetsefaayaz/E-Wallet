using Account.Domain.ValueObjects;

namespace Account.Application.Dtos;

public record CreateAccountDto(
    List<GetCardDto> Cards,
    List<TransactionDto> Transactions);