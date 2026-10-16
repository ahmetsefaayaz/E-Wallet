using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using Identity.Api.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Identity.Register;

internal record RegisterCommand(string UserName, string Email, string Password, string ConfirmPassword)
    : ICommand<RegisterCommandResult>;

internal record RegisterCommandResult(Guid Id);

internal class RegisterCommandHandler(UserManager<User> userManager):ICommandHandler<RegisterCommand, RegisterCommandResult>
{
    public async Task<RegisterCommandResult> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var existingUser = await userManager.FindByEmailAsync(command.Email);
        if (existingUser is not null) throw new BadRequestException("Email already exists", command.Email);
        
        if(!command.Password.Equals(command.ConfirmPassword))
            throw new BadRequestException("Passwords do not match",  command.Password);
        var user = new User
        {
            UserName = command.UserName,
            Email = command.Email
        };
        var result = await userManager.CreateAsync(user, command.Password);
        if (!result.Succeeded) throw new InternalServerException("User could not created");
        return new RegisterCommandResult(user.Id);
    }
}