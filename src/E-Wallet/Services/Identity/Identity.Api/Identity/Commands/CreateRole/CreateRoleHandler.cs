using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using FluentValidation;
using Identity.Api.Entities;
using Identity.Api.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Identity.Commands.CreateRole;

public record CreateRoleCommand(string RoleName) : ICommand<CreateRoleResult>;

public record CreateRoleResult(bool IsSuccess);

internal sealed class CreateRoleValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleValidator()
    {
        RuleFor(r => r.RoleName)
            .NotEmpty()
            .WithMessage("Please specify a role name")
            .MinimumLength(3)
            .WithMessage("3 digits required");
        
    }
}
internal sealed class CreateRoleCommandHandler(RoleManager<Role> roleManager)
    : ICommandHandler<CreateRoleCommand, CreateRoleResult>
{
    public async Task<CreateRoleResult> Handle(CreateRoleCommand command, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByNameAsync(command.RoleName);
        if (role is not null)
            throw new RoleNotFoundException(command.RoleName);
        var result = await roleManager.CreateAsync(new Role
            { Name = command.RoleName, NormalizedName = command.RoleName.ToUpper() });
        if (!result.Succeeded)
            throw new InternalServerException("role creation failed");
        return new CreateRoleResult(true);
    }
}