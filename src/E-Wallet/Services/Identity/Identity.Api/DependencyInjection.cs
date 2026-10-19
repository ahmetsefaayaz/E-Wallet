using System.Security.Claims;
using System.Text;
using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions.Handlers;
using Carter;
using FluentValidation;
using Identity.Api.Data;
using Identity.Api.Data.SeedData;
using Identity.Api.Entities;
using Identity.Api.Identity.GetUserId;
using Identity.Api.Identity.Login;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
        services.AddValidatorsFromAssembly(typeof(Program).Assembly);
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(Program).Assembly);
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddCarter();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        
        services.AddScoped<IJwtCreator,  JwtCreator>();
        services.AddScoped<IGetUserId, GetUserId>();
        
        
        services.AddHttpContextAccessor();
        services.AddIdentityCore<User>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;
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

        services.AddAuthorization(opt =>
        {
            opt.AddPolicy("IsAdmin", policy => policy.RequireRole("Admin"));
            
            opt.AddPolicy("AccountDepartment", policy => policy.RequireAssertion(context =>
                context.User.HasClaim("Department", "Account") || context.User.IsInRole("Admin")));
            opt.AddPolicy("IdentityDepartment", policy => policy.RequireAssertion(context =>
                context.User.HasClaim("Department", "Identity") || context.User.IsInRole("Admin")));
            opt.AddPolicy("NotificationDepartment", policy => policy.RequireAssertion(context =>
                context.User.HasClaim("Department", "Notification") || context.User.IsInRole("Admin")));
        });
        return services;
    }

    public static WebApplication UseIdentityServices(this WebApplication app)
    {
        app.UseExceptionHandler(options => {});
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapCarter();
        return app;
    }
}