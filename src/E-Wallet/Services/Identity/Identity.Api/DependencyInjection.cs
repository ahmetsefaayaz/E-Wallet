using System.Reflection;
using System.Text;
using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions.Handlers;
using BuildingBlocks.Messaging.MassTransit;
using Carter;
using FluentValidation;
using Identity.Api.Data;
using Identity.Api.Entities;
using Identity.Api.Identity.Queries.GetUserId;
using Identity.Api.Identity.Commands.Login;
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
        services.AddValidatorsFromAssembly(typeof(Program).Assembly);
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(Program).Assembly);
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        services.AddMessageBroker(configuration, Assembly.GetExecutingAssembly());
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
        
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<IdentityServiceDbContext>();
            dbContext.Database.Migrate();
        }
        return app;
    }
}