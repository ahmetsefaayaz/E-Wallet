using System.Text;
using Carter;
using Identity.Api.Data;
using Identity.Api.Entities;
using Identity.Api.Identity.Login;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IdentityServiceDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("Database"));
        });
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(Program).Assembly);
        });
        
        services.AddScoped<IJwtCreator,  JwtCreator>();
        
        
        services.AddIdentityCore<User>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
            })
            .AddRoles<Role>()
            .AddEntityFrameworkStores<IdentityServiceDbContext>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            var validAudiences = configuration.GetSection("Jwt:Audiences").Get<string[]>();
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudiences =validAudiences,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
            };
        });

        services.AddAuthorization();
        return services;
    }

    public static WebApplication UseIdentityServices(this WebApplication app)
    {
        
        app.MapCarter();
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }
}