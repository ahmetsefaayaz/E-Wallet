using Account.Application.Dtos;
using Account.Domain.ValueObjects;
using BuildingBlocks.CQRS;
using FluentValidation;

namespace Account.Application.Account.Commands.UpdateCard;

public record UpdateCardCommand(Guid AccountId, Guid CardId, Guid CurrentUserId, UpdateCardDto Card): ICommand<UpdateCardResult>;
public record UpdateCardResult(bool IsSuccess);

public class UpdateCardValidator : AbstractValidator<UpdateCardCommand>
{
    public UpdateCardValidator()
    {
        RuleFor(c => c.AccountId).NotNull().NotEmpty().WithMessage("Please specify an account id");
        RuleFor(c => c.CardId).NotNull().NotEmpty().WithMessage("Please specify a card id");
        RuleFor(c => c.Card.CardHolderName).NotNull().NotEmpty().WithMessage("Please specify a Card Holder Name");
        RuleFor(c => c.Card.CardNumber).NotNull().NotEmpty().WithMessage("Please specify a Card Number");
        RuleFor(c => c.Card.CardExpiryDate).NotNull().NotEmpty().WithMessage("Please specify a Card Expiry Date");
        RuleFor(c => c.Card.CardSecurityNumber).NotNull().NotEmpty().WithMessage("Please specify a Card Security Number");
        
    }
}