namespace Notification.Api.Interfaces;

public interface INotificationRepository
{
    Task AddAsync(Entities.Notification notification, CancellationToken cancellationToken = default);
    Task <Entities.Notification> GetByIdAsync(string notificationId, CancellationToken cancellationToken = default);
    Task <IEnumerable<Entities.Notification>> GetAllAsync(CancellationToken cancellationToken = default);
    Task <IEnumerable<Entities.Notification>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task MarkAsReadAsync(Entities.Notification notification, CancellationToken cancellationToken = default);
}