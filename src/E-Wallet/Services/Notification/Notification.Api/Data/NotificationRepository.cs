
using MongoDB.Driver;
using Notification.Api.Interfaces;

namespace Notification.Api.Data;


public class NotificationRepository: INotificationRepository
{
    private readonly IMongoCollection<Entities.Notification> _notifications;

    public NotificationRepository(IConfiguration configuration)
    {
        var client = new MongoClient(configuration.GetConnectionString("MongoDb"));
        var database = client.GetDatabase("DatabaseSettings:NotificationDbName");
        _notifications = database.GetCollection<Entities.Notification>("Notifications");
    }
    public async Task AddAsync(Entities.Notification notification, CancellationToken cancellationToken = default)
    {
        await _notifications.InsertOneAsync(notification, cancellationToken);
    }

    public async Task<Entities.Notification> GetByIdAsync(string notificationId, CancellationToken cancellationToken = default)
    {
        return await _notifications.Find(Builders<Entities.Notification>.Filter
            .Eq("_id", notificationId)).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Entities.Notification>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await  _notifications.Find(_ => true).ToListAsync(cancellationToken);
    }
}