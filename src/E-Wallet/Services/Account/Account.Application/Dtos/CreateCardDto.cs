using Account.Domain.ValueObjects;

namespace Account.Application.Dtos;

public record CreateCardDto(
    Guid AccountId,
    string CardHolderName,
    string CardNumber,
    string CardExpiryDate,
    string  CardSecurityNumber);