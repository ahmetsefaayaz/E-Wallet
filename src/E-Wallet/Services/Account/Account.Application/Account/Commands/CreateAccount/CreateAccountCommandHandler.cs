using Account.Application.Interfaces;
using Account.Domain.ValueObjects;
using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Account.Application.Account.Commands.CreateAccount;

public class CreateAccountCommandHandler(IAccountDbContext dbContext)
    : ICommandHandler<CreateAccountCommand, CreateAccountResult>
{
    public async Task<CreateAccountResult> Handle(CreateAccountCommand command, CancellationToken cancellationToken)
    {
        var userId = UserId.Of(command.UserId);
        if(await dbContext.Accounts.CountAsync(a => a.UserId == userId, cancellationToken) >= 3)
        {
            throw new BadRequestException("User Cannot Create More Than 3 Accounts", "User cannot have more than 3 accounts");
        }
        var account = new Domain.Models.Account(userId);
        await dbContext.Accounts.AddAsync(account, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateAccountResult(account.Id.Value);
    }
}