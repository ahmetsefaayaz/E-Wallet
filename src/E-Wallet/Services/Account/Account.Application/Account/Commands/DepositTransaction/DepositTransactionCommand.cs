using Account.Domain.ValueObjects;
using BuildingBlocks.CQRS;
using FluentValidation;

namespace Account.Application.Account.Commands.DepositTransaction;

public record DepositTransactionCommand(Guid AccountId, Guid CardId, decimal Amount): ICommand<DepositTransactionResult>;

public class DepositTransactionValidator : AbstractValidator<DepositTransactionCommand>
{
    public DepositTransactionValidator()
    {
        RuleFor(c => c.AccountId.ToString()).NotNull().NotEmpty().WithMessage("Account Id is required")
            .Length(10, 100).WithMessage("Account Id must be 10 characters or less than 100 characters");
        RuleFor(c => c.CardId).NotNull().NotEmpty().WithMessage("CardId is required");
        RuleFor(c => c.Amount).NotNull().NotEmpty().WithMessage("Amount is required")
            .GreaterThan(0).WithMessage("Amount must be greater than 0")
            .LessThan(1000000).WithMessage("Amount must be less than 1000000");
    }
}

public record DepositTransactionResult(bool IsSuccess);
