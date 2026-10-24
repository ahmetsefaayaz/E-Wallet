using Account.Application.Exceptions;
using Account.Application.Interfaces;
using Account.Domain.ValueObjects;
using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;

namespace Account.Application.Account.Commands.DepositTransaction;

public class DepositTransactionCommandHandler(IAccountDbContext dbContext)
    : ICommandHandler<DepositTransactionCommand, DepositTransactionResult>
{
    public async Task<DepositTransactionResult> Handle(DepositTransactionCommand command, CancellationToken cancellationToken)
    {
        var accountId = AccountId.Of(command.AccountId);
        var account = await dbContext.Accounts
                          .Include(a => a.Cards)
                          .FirstOrDefaultAsync(a => a.Id == accountId, cancellationToken)
            ?? throw new AccountNotFoundException(command.AccountId.ToString());
        if (!account.Cards.Any(c => c.Id == CardId.Of(command.CardId)))
            throw new CardNotFoundException(command.CardId.ToString());
        account.CreateTransaction(CardId.Of(command.CardId), TransactionType.Deposit, command.Amount);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new DepositTransactionResult(true);
    }
}