using Account.Domain.ValueObjects;

namespace Account.Application.Dtos;

public record UpdateCardDto(
    string CardHolderName,
    string CardNumber,
    string CardExpiryDate,
    string  CardSecurityNumber);