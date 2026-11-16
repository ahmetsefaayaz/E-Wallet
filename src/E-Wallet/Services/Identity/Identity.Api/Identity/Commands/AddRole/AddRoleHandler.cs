using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using FluentValidation;
using Identity.Api.Entities;
using Identity.Api.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Identity.Api.Identity.Commands.AddRole;


public record AddRoleCommand(Guid UserId, string RoleName): ICommand<AddRoleResult>;

internal sealed class AddRoleValidator : AbstractValidator<AddRoleCommand>
{
    public AddRoleValidator()
    {
        RuleFor(r => r.UserId).NotNull();
        RuleFor(r => r.RoleName)
            .NotEmpty()
            .WithMessage("Please specify a role name")
            .MinimumLength(3)
            .WithMessage("3 digits required");
        
    }
}
public record AddRoleResult(bool IsSuccess);


internal sealed class AddRoleCommandHandler(UserManager<User> userManager)
    : ICommandHandler<AddRoleCommand, AddRoleResult>
{
    public async Task<AddRoleResult> Handle(AddRoleCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user == null)
            throw new UserNotFoundException();

        var result = await userManager.AddToRoleAsync(user, command.RoleName);

        if (!result.Succeeded)
            throw new InternalServerException("role could not added to the user");
        return new AddRoleResult(true);
    }
}