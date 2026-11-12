using BuildingBlocks.Messaging.Events;
using Identity.Api.Entities;
using MassTransit;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Data.SeedData;

public static class SeedData
{
    public static async Task Initialize(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityServiceDbContext>();
        await AddRoles(roleManager, userManager);
        await AddAdmin(userManager, publishEndpoint, dbContext);
    }

    private static async Task AddRoles(RoleManager<Role> roleManager, UserManager<User> userManager)
    {
        if (!roleManager.Roles.Any())
        {
            var roles = new[] { "Admin", "User" };
            foreach (var role in roles)
            {
                await roleManager.CreateAsync(new Role{ Name = role, NormalizedName = role.ToUpper() });
            }
        }
    }

    private static async Task AddAdmin(UserManager<User> userManager, IPublishEndpoint publishEndpoint, IdentityServiceDbContext dbContext)
    {
        if (!userManager.Users.Any())
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            var adminMail = "admin@mail.com";
            var admin = await userManager.FindByEmailAsync(adminMail);
            if(admin is not null)
                return;
            var adminUser = new User
            {
                Email = adminMail,
                UserName = "SuperUser"
            };
            var result = await userManager.CreateAsync(adminUser, "AdminPassword0");
            if (!result.Succeeded) throw new Exception("Failed to create admin user");
            await userManager.AddToRoleAsync(adminUser, "Admin");
            var integrationEvent = new UserRegisteredIntegrationEvent
            {
                UserId = adminUser.Id,
                Email = adminUser.Email,
                Username = adminUser.UserName
            };
            await publishEndpoint.Publish(integrationEvent);
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            
        }
    }
}