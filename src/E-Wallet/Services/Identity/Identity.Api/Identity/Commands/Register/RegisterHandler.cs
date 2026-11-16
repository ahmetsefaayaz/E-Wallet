using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Messaging.Events;
using FluentValidation;
using Identity.Api.Data;
using Identity.Api.Entities;
using MassTransit;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Identity.Commands.Register;

public record RegisterCommand(string UserName, string Email, string Password, string ConfirmPassword)
    : ICommand<RegisterCommandResult>;

public record RegisterCommandResult(Guid Id);

internal sealed class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(r => r.UserName).NotEmpty().WithMessage("Username is required")
            .MaximumLength(128).WithMessage("Username cannot exceed 128");
        RuleFor(r => r.Email).NotEmpty().WithMessage("Email is required")
            .MaximumLength(128).WithMessage("Email cannot exceed 128");
        RuleFor(p => p.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8  characters long")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.");
        RuleFor(r => r.ConfirmPassword)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8  characters long")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.");
    }
}


internal sealed class RegisterCommandHandler (
    UserManager<User> userManager,
    IPublishEndpoint publishEndpoint, 
    IdentityServiceDbContext dbContext) 
    :ICommandHandler<RegisterCommand, RegisterCommandResult>
{
    public async Task<RegisterCommandResult> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var existingUser = await userManager.FindByEmailAsync(command.Email);
        if (existingUser is not null) throw new BadRequestException("Email already exists", command.Email);
        
        if(!command.Password.Equals(command.ConfirmPassword))
            throw new BadRequestException("Passwords do not match",  command.Password);
        await using var transaction =
            await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var user = new User
        {
            UserName = command.UserName,
            Email = command.Email
        };
        var result = await userManager.CreateAsync(user, command.Password);
        if (!result.Succeeded) throw new InternalServerException("User could not created");
        await userManager.AddToRoleAsync(user, "User");
        var integrationEvent = new UserRegisteredIntegrationEvent
        {
            UserId = user.Id,
            Email = user.Email,
            Username = user.UserName
        };
        await publishEndpoint.Publish(integrationEvent, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return new RegisterCommandResult(user.Id);
    }
}