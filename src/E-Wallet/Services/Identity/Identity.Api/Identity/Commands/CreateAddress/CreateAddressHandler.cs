using BuildingBlocks.CQRS;
using Identity.Api.Data;
using Identity.Api.Dtos;
using Identity.Api.Entities;
using Identity.Api.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Identity.Commands.CreateAddress;

internal record CreateAddressCommand(Guid UserId, AddressDto Address) : ICommand<CreateAddressResult>;

internal record CreateAddressResult(bool IsSuccess);


internal class CreateAddressCommandHandler(IdentityServiceDbContext dbContext)
    : ICommandHandler<CreateAddressCommand, CreateAddressResult>
{
    public async Task<CreateAddressResult> Handle(CreateAddressCommand command, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken)
            ?? throw new UserNotFoundException();
        var address = new Address
        {
            AddressLine = command.Address.AddressLine,
            City = command.Address.City,
            State = command.Address.State,
            Country = command.Address.Country,
            ZipCode = command.Address.ZipCode
        };
        user.SetAddress(address);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateAddressResult(true);
    }
}