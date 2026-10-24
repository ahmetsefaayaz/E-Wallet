using Account.Domain.ValueObjects;

namespace Account.Application.Dtos;

public record TransactionDto(
    Guid Id,
    Guid AccountId,
    Guid? CardId,
    TransactionType TransactionType,
    Status Status,
    decimal Amount);