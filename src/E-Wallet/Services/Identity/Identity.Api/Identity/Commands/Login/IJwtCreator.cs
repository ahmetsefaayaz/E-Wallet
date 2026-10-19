using Identity.Api.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Identity.Commands.Login;

public interface IJwtCreator
{
    Task<string> GenerateJwtToken(UserManager<User> userManager, User user, IConfiguration configuration);
}