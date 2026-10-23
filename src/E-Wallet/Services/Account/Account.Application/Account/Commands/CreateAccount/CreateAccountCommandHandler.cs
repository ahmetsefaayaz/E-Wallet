using Account.Application.Interfaces;
using Account.Domain.ValueObjects;
using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;

namespace Account.Application.Account.Commands.CreateAccount;

public class CreateAccountCommandHandler(IAccountDbContext dbContext)
    : ICommandHandler<CreateAccountCommand, CreateAccountResult>
{
    public async Task<CreateAccountResult> Handle(CreateAccountCommand command, CancellationToken cancellationToken)
    {
        var userId = UserId.Of(command.UserId);
        var account = new Domain.Models.Account(userId);
        await dbContext.Accounts.AddAsync(account, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateAccountResult(account.Id.Value);
    }
}