using MongoDB.Bson.Serialization.Attributes;
using Notification.Api.Entities.Enums;

namespace Notification.Api.Entities;

public class Notification
{
    public Notification(Guid userId, Guid accountId, string title, string message, string type, NotificationChannel channel,
        Guid? referenceId, DateTime createdAt)
    {
        UserId = userId;
        AccountId = accountId;
        Title = title;
        Message = message;
        Type = type;
        Channel = channel;
        ReferenceId = referenceId;
        CreatedAt = createdAt;
    }
    [BsonId]
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public Guid AccountId { get; private set; }
    
    public string Title { get; private set; } = default!;
    public string Message { get; private set; } = default!;
    
    public string Type { get; private set; } 
    
    public NotificationChannel Channel { get; private set; }
    public bool IsRead { get; set; } = false;
    
    public Guid? ReferenceId { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
}