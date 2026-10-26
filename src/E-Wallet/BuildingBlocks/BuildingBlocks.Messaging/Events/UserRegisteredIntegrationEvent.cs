namespace BuildingBlocks.Messaging.Events;

public class UserRegisteredIntegrationEvent: IntegrationEvent
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
}