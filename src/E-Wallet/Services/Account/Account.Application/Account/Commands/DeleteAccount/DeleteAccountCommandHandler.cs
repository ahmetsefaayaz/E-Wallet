using Account.Application.Exceptions;
using Account.Application.Interfaces;
using Account.Domain.ValueObjects;
using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Account.Application.Account.Commands.DeleteAccount;

internal sealed class DeleteAccountCommandHandler(IAccountDbContext dbContext)
    : ICommandHandler<DeleteAccountCommand, DeleteAccountResult>
{
    public async Task<DeleteAccountResult> Handle(DeleteAccountCommand command, CancellationToken cancellationToken)
    {
        if (!command.IsAdmin)
            throw new ForbiddenAccessException("Only administrators can delete accounts.");
        var accountId = AccountId.Of(command.AccountId);
        var account = await dbContext.Accounts.FirstOrDefaultAsync(
                a => a.Id == accountId, cancellationToken)
            ?? throw new AccountNotFoundException(accountId.Value.ToString());
        dbContext.Accounts.Remove(account);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new DeleteAccountResult(true);
    }
}