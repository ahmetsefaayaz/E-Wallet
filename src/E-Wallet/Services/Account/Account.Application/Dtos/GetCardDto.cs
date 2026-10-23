using Account.Domain.ValueObjects;

namespace Account.Application.Dtos;

public record GetCardDto(
    Guid Id,
    Guid AccountId,
    string CardHolderName,
    string CardNumber,
    string CardExpiryDate,
    string  CardSecurityNumber);