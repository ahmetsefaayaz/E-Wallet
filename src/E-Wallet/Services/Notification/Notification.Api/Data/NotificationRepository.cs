
using MongoDB.Driver;
using Notification.Api.Interfaces;

namespace Notification.Api.Data;


public class NotificationRepository: INotificationRepository
{
    private readonly IMongoCollection<Entities.Notification> _notifications;

    public NotificationRepository(IConfiguration configuration, IMongoClient client)
    {
        var database = client.GetDatabase(configuration["DatabaseSettings:NotificationDbName"]);
        _notifications = database.GetCollection<Entities.Notification>("Notifications");
    }
    public async Task AddAsync(Entities.Notification notification, CancellationToken cancellationToken = default)
    {
        await _notifications.InsertOneAsync(notification, null, cancellationToken);
    }

    public async Task<Entities.Notification> GetByIdAsync(string notificationId, CancellationToken cancellationToken = default)
    {
        return await _notifications.Find(n => n.Id.ToString() == notificationId).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Entities.Notification> GetByIdAsync(
        string notificationId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _notifications.Find(n =>
            n.Id.ToString() == notificationId && n.UserId.ToString() == userId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Entities.Notification>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await  _notifications.Find(_ => true).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Entities.Notification>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _notifications.Find(n => n.UserId.ToString() ==  userId).ToListAsync(cancellationToken);
    }

    public async Task MarkAsReadAsync(Entities.Notification notification, CancellationToken cancellationToken = default)
    {
        notification.IsRead = true;
        await _notifications.ReplaceOneAsync(n => n.Id == notification.Id, notification, cancellationToken: cancellationToken);
    }
}