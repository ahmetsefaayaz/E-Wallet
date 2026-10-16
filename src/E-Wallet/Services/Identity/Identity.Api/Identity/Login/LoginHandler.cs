using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using Identity.Api.Entities;
using Identity.Api.Exceptions;
using Mapster;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Identity.Login;


internal record LoginCommand(string Email, string Password): ICommand<LoginResult>;

internal record LoginResult(string Token);

internal class LoginCommandHandler(UserManager<User> userManager, IConfiguration configuration, IJwtCreator jwtCreator)
    : ICommandHandler<LoginCommand, LoginResult>
{
    public async Task<LoginResult> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(command.Email);
        if (user is null) throw new UserNotFoundException(command.Email);

        if (!await userManager.CheckPasswordAsync(user, command.Password))
            throw new BadRequestException("wrong password", command.Password);
        var token = await jwtCreator.GenerateJwtToken(userManager, user, configuration);
        return new LoginResult(token);
    }
}
