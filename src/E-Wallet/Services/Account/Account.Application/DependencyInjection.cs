using System.Reflection;
using BuildingBlocks.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Account.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
            configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });
        
        return services;
    }
}