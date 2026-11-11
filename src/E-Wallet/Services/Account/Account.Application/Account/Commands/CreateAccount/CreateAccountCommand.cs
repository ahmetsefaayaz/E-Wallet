using Account.Domain.ValueObjects;
using BuildingBlocks.CQRS;
using FluentValidation;

namespace Account.Application.Account.Commands.CreateAccount;

public record CreateAccountCommand(Guid CurrentUserId, string IdempotencyKey): IIdempotentCommand<CreateAccountResult>;

public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountCommandValidator()
    {
        RuleFor(c => c.CurrentUserId).NotEmpty().WithMessage("Current user id cannot be empty");
        RuleFor(c => c.IdempotencyKey).NotEmpty().WithMessage("Idempotency key cannot be empty");
    }
}

public record CreateAccountResult(Guid AccountId);