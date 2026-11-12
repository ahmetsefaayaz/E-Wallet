using Account.Application.Exceptions;
using Account.Application.Interfaces;
using Account.Domain.ValueObjects;
using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;

namespace Account.Application.Account.Commands.WithdrawTransaction;

public class WithdrawTransactionCommandHandler(IAccountDbContext dbContext, IDomainEventDispatcher domainEventDispatcher):
    ICommandHandler<WithdrawTransactionCommand, WithdrawTransactionResult>
{
    public async Task<WithdrawTransactionResult> Handle(WithdrawTransactionCommand command, CancellationToken cancellationToken)
    {
        var accountId = AccountId.Of(command.AccountId);
        var account = await dbContext.Accounts
            .Include(a => a.Cards)
            .FirstOrDefaultAsync(a => a.Id == accountId
                && a.UserId == UserId.Of(command.CurrentUserId), cancellationToken)
                ?? throw new AccountNotFoundException(command.AccountId.ToString());
        if (!account.Cards.Any(c => c.Id == CardId.Of(command.CardId)))
            throw new CardNotFoundException(command.CardId.ToString());
        account.CreateTransaction(CardId.Of(command.CardId), TransactionType.Withdrawal, command.Amount);
        await domainEventDispatcher.DispatchAsync(cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new WithdrawTransactionResult(true);
    }
}