using Account.Application.Exceptions;
using Account.Application.Interfaces;
using Account.Domain.ValueObjects;
using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;

namespace Account.Application.Account.Commands.DeleteCard;

internal sealed class DeleteCardCommandHandler(IAccountDbContext dbContext):
    ICommandHandler<DeleteCardCommand, DeleteCardResult>
{
    public async Task<DeleteCardResult> Handle(DeleteCardCommand command, CancellationToken cancellationToken)
    {
        var accountId = AccountId.Of(command.AccountId);
        var account = await dbContext.Accounts
            .Include(a => a.Cards)
            .FirstOrDefaultAsync(a => a.Id == accountId
                && a.UserId == UserId.Of(command.CurrentUserId), cancellationToken)
            ?? throw new AccountNotFoundException(command.AccountId.ToString());
        account.RemoveCard(CardId.Of(command.CardId));
        await dbContext.SaveChangesAsync(cancellationToken);
        return new DeleteCardResult(true);
    }
}