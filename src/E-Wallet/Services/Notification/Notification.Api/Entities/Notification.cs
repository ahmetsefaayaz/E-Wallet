using Notification.Api.Entities.Enums;

namespace Notification.Api.Entities;

public class Notification
{
    public Notification(Guid accountId, string title, string message, NotificationType type, NotificationChannel channel,
        Guid? referenceId, DateTime createdAt)
    {
        AccountId = accountId;
        Title = title;
        Message = message;
        Type = type;
        Channel = channel;
        ReferenceId = referenceId;
        CreatedAt = createdAt;
    }
    public Guid Id => Guid.NewGuid();
    public Guid AccountId { get; private set; }
    
    public string Title { get; private set; } = default!;
    public string Message { get; private set; } = default!;
    
    public NotificationType Type { get; private set; } 
    
    public NotificationChannel Channel { get; private set; }
    
    
    public Guid? ReferenceId { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
}