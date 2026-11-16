using Account.Application.Exceptions;
using Account.Application.Interfaces;
using Account.Domain.ValueObjects;
using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;

namespace Account.Application.Account.Commands.UpdateCard;

internal sealed class UpdateCardCommandHandler(IAccountDbContext dbContext)
    : ICommandHandler<UpdateCardCommand, UpdateCardResult>
{
    public async Task<UpdateCardResult> Handle(UpdateCardCommand command, CancellationToken cancellationToken)
    {
        
        var accountId = AccountId.Of(command.AccountId);
        var account = await dbContext.Accounts
                          .Include(t => t.Cards)
                          .FirstOrDefaultAsync(a => a.Id == accountId
                              && a.UserId == UserId.Of(command.CurrentUserId), cancellationToken)
            ?? throw new AccountNotFoundException(command.AccountId.ToString());
        account.UpdateCard(CardId.Of(command.CardId), CardHolderName.Of(command.Card.CardHolderName), CardNumber.Of(command.Card.CardNumber),
            CardExpiryDate.Of(command.Card.CardExpiryDate), CardSecurityNumber.Of(command.Card.CardSecurityNumber));
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateCardResult(true);
    }
}