using System.Reflection;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Messaging.MassTransit;

public static class Extensions
{
    public static IServiceCollection AddMessageBroker<TDbContext>(this IServiceCollection services,
        IConfiguration configuration, Assembly? assembly = null)
    where TDbContext: DbContext
    {
        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();
            if(assembly != null)
                config.AddConsumers(assembly);
            config.AddEntityFrameworkOutbox<TDbContext>(o =>
            {
                o.UsePostgres();
                o.UseBusOutbox();
            });
            config.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(configuration["MessageBroker:Host"]!), h =>
                {
                    h.Username(configuration["MessageBroker:Username"]!);
                    h.Password(configuration["MessageBroker:Password"]!);
                });
                cfg.ConfigureEndpoints(context);
                cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
            });
        });
        return services;
    }
    public static IServiceCollection AddMessageBroker(this IServiceCollection services,
        IConfiguration configuration, Assembly? assembly = null)
    {
        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();
            if(assembly != null)
                config.AddConsumers(assembly);
            config.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(configuration["MessageBroker:Host"]!), h =>
                {
                    h.Username(configuration["MessageBroker:Username"]!);
                    h.Password(configuration["MessageBroker:Password"]!);
                });
                cfg.ConfigureEndpoints(context);
                cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
            });
        });
        return services;
    }
}