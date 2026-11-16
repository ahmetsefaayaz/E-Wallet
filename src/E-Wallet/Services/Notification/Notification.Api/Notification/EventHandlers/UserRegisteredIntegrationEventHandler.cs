using BuildingBlocks.Messaging.Events;
using MassTransit;
using Notification.Api.Entities;
using Notification.Api.Interfaces;

namespace Notification.Api.Notification.EventHandlers;

public sealed class UserRegisteredIntegrationEventHandler(IUserRepository repository, 
    ILogger<UserRegisteredIntegrationEventHandler> logger)
    : IConsumer<UserRegisteredIntegrationEvent>
{
    public async Task Consume(ConsumeContext<UserRegisteredIntegrationEvent> context)
    {
        var user = new User
        {
            Email = context.Message.Email,
            Username = context.Message.Username,
            UserId = context.Message.UserId
        };
        await repository.AddAsync(user);
    }
}