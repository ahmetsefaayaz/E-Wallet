using Account.Domain.ValueObjects;
using BuildingBlocks.CQRS;
using FluentValidation;

namespace Account.Application.Account.Commands.DeleteAccount;

public record DeleteAccountCommand(Guid AccountId, Guid CurrentUserId, bool IsAdmin): ICommand<DeleteAccountResult>;

public class DeleteAccountCommandValidator : AbstractValidator<DeleteAccountCommand>
{
    public DeleteAccountCommandValidator()
    {
        RuleFor(c => c.AccountId).NotNull().NotEmpty().WithMessage("Account Id cannot be null or empty");
    }
}
public record DeleteAccountResult(bool IsSuccess);