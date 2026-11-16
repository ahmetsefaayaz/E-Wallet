using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using FluentValidation;
using Identity.Api.Data;
using Identity.Api.Dtos;
using Identity.Api.Entities;
using Identity.Api.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Identity.Commands.CreateAddress;

public record CreateAddressCommand(Guid UserId, AddressDto Address) : ICommand<CreateAddressResult>;

public record CreateAddressResult(bool IsSuccess);

internal sealed class CreateAddressValidator : AbstractValidator<CreateAddressCommand>
{
    public CreateAddressValidator()
    {
        RuleFor(a => a.Address.AddressLine)
            .NotEmpty()
            .WithMessage("Please specify a valid address line")
            .MinimumLength(3)
            .WithMessage("3 digits required")
            .MaximumLength(100)
            .WithMessage("AddressLine cannot exceed 100 digits");
        RuleFor(a => a.Address.Country)
            .NotEmpty()
            .WithMessage("Please specify a valid Country name")
            .MinimumLength(3)
            .WithMessage("3 digits required")
            .MaximumLength(100)
            .WithMessage("Country cannot exceed 100 digits");
        RuleFor(a => a.Address.ZipCode)
            .NotEmpty()
            .WithMessage("Please specify a valid ZipCode")
            .MinimumLength(3)
            .WithMessage("3 digits required")
            .MaximumLength(100)
            .WithMessage("ZipCode cannot exceed 100 digits");
        RuleFor(a => a.Address.City)
            .NotEmpty()
            .WithMessage("Please specify a valid City")
            .MinimumLength(3)
            .WithMessage("3 digits required")
            .MaximumLength(100)
            .WithMessage("City cannot exceed 100 digits");
        RuleFor(a => a.Address.State)
            .NotEmpty()
            .WithMessage("Please specify a valid State")
            .MinimumLength(3)
            .WithMessage("3 digits required")
            .MaximumLength(100)
            .WithMessage("State cannot exceed 100 digits");

    }
}

internal sealed class CreateAddressCommandHandler(IdentityServiceDbContext dbContext)
    : ICommandHandler<CreateAddressCommand, CreateAddressResult>
{
    public async Task<CreateAddressResult> Handle(CreateAddressCommand command, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken)
            ?? throw new UserNotFoundException();
        if(user.Address != null)
        {
            throw new BadRequestException("Address Already Exists", "User already has an address. Please update the existing address instead.");
        }
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