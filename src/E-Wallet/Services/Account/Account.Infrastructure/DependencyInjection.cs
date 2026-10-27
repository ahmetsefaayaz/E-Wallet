using System.Text;
using Account.Application.Interfaces;
using Account.Infrastructure.Data;
using Account.Infrastructure.Data.Interceptors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Account.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        
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
                ValidAudiences = validAudiences,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
            };
        });

        services.AddAuthorization(opt =>
        {
            opt.AddPolicy("IsAdmin", policy => policy.RequireRole("Admin"));
            
            opt.AddPolicy("AccountDepartment", policy => policy.RequireAssertion(context =>
                context.User.HasClaim("Department", "Account") || context.User.IsInRole("Admin")));
        });
        
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, AuditableInterceptor>();
        services.AddScoped<IAccountDbContext, AccountDbContext>();
        services.AddDbContext<AccountDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(configuration.GetConnectionString("Database"));
        });
        
        return services;
    }
}