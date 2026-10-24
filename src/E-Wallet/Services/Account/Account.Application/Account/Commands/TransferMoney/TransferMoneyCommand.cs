using Account.Domain.ValueObjects;
using BuildingBlocks.CQRS;
using FluentValidation;

namespace Account.Application.Account.Commands.TransferMoney;

public record TransferMoneyCommand(Guid AccountId, Guid ReceiverAccountId,
    Guid CardId, decimal Amount): ICommand<OutgoingTransactionResult>;

public class OutgoingTransactionValidator : AbstractValidator<TransferMoneyCommand>
{
    public OutgoingTransactionValidator()
    {
        RuleFor(c => c.AccountId.ToString())
            .NotNull().NotEmpty().WithMessage("Account Id is required")
            .Length(10, 100).WithMessage("Account Id must be 10 characters or less than 100 characters");
        RuleFor(c => c.ReceiverAccountId.ToString())
            .NotNull().NotEmpty().WithMessage("Receiver Account Id is required")
            .Length(10, 100).WithMessage("Account Id must be 10 characters or less than 100 characters");
        RuleFor(c => c.CardId).NotNull().NotEmpty().WithMessage("CardId is required");
        RuleFor(c => c.Amount).NotNull().NotEmpty().WithMessage("Amount is required")
            .GreaterThan(0).WithMessage("Amount must be greater than 0")
            .LessThan(1000000).WithMessage("Amount must be less than 1000000");
    }
}
public record OutgoingTransactionResult(bool IsSuccess);