using Account.Domain.ValueObjects;
using BuildingBlocks.CQRS;
using FluentValidation;

namespace Account.Application.Account.Commands.CreateAccount;

public record CreateAccountCommand(Guid UserId): ICommand<CreateAccountResult>;

public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountCommandValidator()
    {
        RuleFor(c => c.UserId).NotNull().NotEmpty().WithMessage("UserId cannot be empty");
    }
}

public record CreateAccountResult(Guid Id);