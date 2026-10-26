using Notification.Api.Entities;

namespace Notification.Api.Interfaces;

public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task<User> GetAsync(Guid id, CancellationToken cancellationToken = default);
}