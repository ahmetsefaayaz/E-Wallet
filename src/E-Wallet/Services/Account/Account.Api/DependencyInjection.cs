using BuildingBlocks.Exceptions.Handlers;
using Carter;

namespace Account.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddCarter();
        services.AddAuthentication();
        services.AddAuthorization();
        return services;
    }

    public static WebApplication UseApiServices(this WebApplication app)
    {
        app.UseExceptionHandler(opt => {});
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapCarter();
        return app;
    }
}