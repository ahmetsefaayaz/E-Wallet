using System.Reflection;
using Account.Application.Idempotency;
using BuildingBlocks.Behaviors;
using BuildingBlocks.Interfaces;
using BuildingBlocks.Messaging.MassTransit;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Account.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {

        services.AddScoped<IIdempotencyService, IdempotencyService>();
        services.AddMessageBroker(config);
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
            configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
            configuration.AddOpenBehavior(typeof(IdempotencyBehavior<,>));
        });
        
        return services;
    }
}