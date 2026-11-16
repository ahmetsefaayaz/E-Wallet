using Account.Application.Dtos;
using Account.Domain.ValueObjects;
using BuildingBlocks.CQRS;
using FluentValidation;

namespace Account.Application.Account.Commands.CreateCard;

public record CreateCardCommand(Guid AccountId, Guid CurrentUserId, CreateCardDto Card): ICommand<CreateCardResult>;
public record CreateCardResult(Guid CardId);

internal sealed class CreateCardCommandValidator : AbstractValidator<CreateCardCommand>
{
    public CreateCardCommandValidator()
    {
        RuleFor(c => c.Card.CardHolderName).NotNull().NotEmpty().WithMessage("Card Holder Name cannot be null or empty");
        RuleFor(c => c.Card.CardNumber).NotNull().NotEmpty().WithMessage("Card Number cannot be null or empty");
        RuleFor(c => c.Card.CardExpiryDate).NotNull().NotEmpty().WithMessage("Card Expiry Date cannot be null or empty");
        RuleFor(c => c.Card.CardSecurityNumber).NotNull().NotEmpty().WithMessage("Card Security Number cannot be null or empty");
        
    }
}