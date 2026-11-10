using Account.Application.Exceptions;
using Account.Application.Interfaces;
using Account.Domain.ValueObjects;
using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Account.Application.Account.Commands.CreateCard;

public class CreateCardCommandHandler(IAccountDbContext dbContext)
    : ICommandHandler<CreateCardCommand, CreateCardResult>
{
    public async Task<CreateCardResult> Handle(CreateCardCommand command, CancellationToken cancellationToken)
    {
        var accountId = AccountId.Of(command.AccountId);
        var account = await dbContext.Accounts
            .Include(a => a.Cards)
            .FirstOrDefaultAsync(a => a.Id == accountId, cancellationToken)
            ?? throw new AccountNotFoundException(command.AccountId.ToString());
        var card = account.CreateCard(CardHolderName.Of(command.Card.CardHolderName), CardNumber.Of(command.Card.CardNumber),
                       CardExpiryDate.Of(command.Card.CardExpiryDate), CardSecurityNumber.Of(command.Card.CardSecurityNumber))
            ?? throw new InternalServerException("Card could not created");
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateCardResult(card.Id.Value);
    }
}