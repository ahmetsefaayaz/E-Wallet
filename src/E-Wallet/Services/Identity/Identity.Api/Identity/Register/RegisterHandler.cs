using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using FluentValidation;
using Identity.Api.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Identity.Register;

public record RegisterCommand(string UserName, string Email, string Password, string ConfirmPassword)
    : ICommand<RegisterCommandResult>;

internal record RegisterCommandResult(Guid Id);

public class RegisterValidator : AbstractValidator<RegisterCommand>
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