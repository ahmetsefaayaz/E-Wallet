using Account.Domain.ValueObjects;
using BuildingBlocks.CQRS;
using FluentValidation;

namespace Account.Application.Account.Commands.DeleteCard;

public record DeleteCardCommand(Guid AccountId, Guid CardId, Guid CurrentUserId) : ICommand<DeleteCardResult>;

internal sealed class DeleteCardCommandValidator : AbstractValidator<DeleteCardCommand>
{
    public DeleteCardCommandValidator()
    {
        RuleFor(c => c.AccountId).NotEmpty().NotNull().WithMessage("Account Id cannot be empty or null");
        RuleFor(c => c.CardId).NotEmpty().NotNull().WithMessage("Card Id cannot be empty or null");
    }
}

public record DeleteCardResult(bool IsSuccess);
