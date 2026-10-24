using Account.Domain.ValueObjects;

namespace Account.Application.Dtos;

public record CreateCardDto(
    string CardHolderName,
    string CardNumber,
    string CardExpiryDate,
    string  CardSecurityNumber);