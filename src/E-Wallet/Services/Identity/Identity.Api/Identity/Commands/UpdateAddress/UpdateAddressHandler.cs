using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using FluentValidation;
using Identity.Api.Data;
using Identity.Api.Dtos;
using Identity.Api.Entities;
using Identity.Api.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Identity.Commands.UpdateAddress;

public record UpdateAddressCommand(Guid UserId, AddressDto Address) : ICommand<UpdateAddressResult>;
public record UpdateAddressResult(bool IsSuccess);

public class UpdateValidator : AbstractValidator<UpdateAddressCommand>
{
    public UpdateValidator()
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

internal class UpdateAddressHandler(IdentityServiceDbContext dbContext)
    : ICommandHandler<UpdateAddressCommand, UpdateAddressResult>
{
    public async Task<UpdateAddressResult> Handle(UpdateAddressCommand command, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .Include(u => u.Address)
            .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken)
            ?? throw new UserNotFoundException();
        if (user.Address is null)
            throw new BadRequestException("Address Not Found", "User does not have an address to update.");
        var newAddress = new Address
        {
            AddressLine = command.Address.AddressLine,
            City = command.Address.City,
            Country = command.Address.Country,
            ZipCode = command.Address.ZipCode,
            State = command.Address.State
        };
        user.SetAddress(newAddress);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateAddressResult(true);
    }
}