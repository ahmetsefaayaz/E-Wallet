using System.Security.Claims;
using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using FluentValidation;
using Identity.Api.Entities;
using Identity.Api.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Identity.Commands.CreateClaim;

public record CreateClaimCommand(Guid UserId, string Type, string Value): ICommand<CreateClaimResult>;

internal sealed class CreateClaimValidator : AbstractValidator<CreateClaimCommand>
{
    public CreateClaimValidator()
    {
        RuleFor(c => c.UserId)
            .NotEmpty()
            .WithMessage("UserId is required");
        RuleFor(c => c.Type)
            .NotEmpty()
            .WithMessage("Type not specified")
            .MinimumLength(3)
            .WithMessage("3 digits required");
        RuleFor(c => c.Value)
            .NotEmpty()
            .WithMessage("Value not specified")
            .MinimumLength(3)
            .WithMessage("3 digits required");
    }
}

public record CreateClaimResult(bool IsSuccess);

internal sealed class CreateClaimCommandHandler(UserManager<User> userManager)
    : ICommandHandler<CreateClaimCommand, CreateClaimResult>
{
    public async Task<CreateClaimResult> Handle(CreateClaimCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());

        if (user == null)
            throw new UserNotFoundException();
        
        var result = await userManager.AddClaimAsync(user, new Claim(command.Type, command.Value));
        
        if (result.Succeeded)
            return new CreateClaimResult(true);
        throw new InternalServerException("Failed to create claim");

    }
}