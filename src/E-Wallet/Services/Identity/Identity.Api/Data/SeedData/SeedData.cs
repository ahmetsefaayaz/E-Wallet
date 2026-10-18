using Identity.Api.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Data.SeedData;

public static class SeedData
{
    public static async Task Initialize(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        await AddRoles(roleManager, userManager);
        await AddAdmin(userManager);
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

    private static async Task AddAdmin(UserManager<User> userManager)
    {
        if (!userManager.Users.Any())
        {
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
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}