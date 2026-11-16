using Account.Application.Exceptions;
using Account.Application.Interfaces;
using Account.Domain.ValueObjects;
using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;

namespace Account.Application.Account.Commands.TransferMoney;

internal sealed class TransferMoneyCommandHandler(IAccountDbContext dbContext, IDomainEventDispatcher domainEventDispatcher)
    : ICommandHandler<TransferMoneyCommand, OutgoingTransactionResult>
{
    public async Task<OutgoingTransactionResult> Handle(TransferMoneyCommand command, CancellationToken cancellationToken)
    {
        var accountId = AccountId.Of(command.AccountId);
        var outgoingAccount = await dbContext.Accounts
            .Include(a => a.Cards)
            .FirstOrDefaultAsync(a => a.Id == accountId
                && a.UserId == UserId.Of(command.CurrentUserId), cancellationToken)
            ?? throw new AccountNotFoundException(accountId.Value.ToString());
        if (!outgoingAccount.Cards.Any(c => c.Id == CardId.Of(command.CardId)))
            throw new CardNotFoundException(command.CardId.ToString());
        var incomingAccountId = AccountId.Of(command.ReceiverAccountId);
        var incomingAccount = await dbContext.Accounts
            .Include(a => a.Cards)
            .FirstOrDefaultAsync(a => a.Id == incomingAccountId, cancellationToken)
            ?? throw new AccountNotFoundException(incomingAccountId.Value.ToString());
        outgoingAccount.CreateTransaction(CardId.Of(command.CardId), TransactionType.OutgoingTransfer, 
            command.Amount);
        incomingAccount.CreateTransaction(null, TransactionType.IncomingTransfer,
            command.Amount);
        await domainEventDispatcher.DispatchAsync(cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new OutgoingTransactionResult(true);
    }
}