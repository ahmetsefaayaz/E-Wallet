using MongoDB.Bson.Serialization.Attributes;

namespace Notification.Api.Entities;

public class User
{
    [BsonId]
    public Guid Id{get; set;} = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
}