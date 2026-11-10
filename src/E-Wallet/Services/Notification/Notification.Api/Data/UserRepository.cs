using MongoDB.Driver;
using Notification.Api.Entities;
using Notification.Api.Interfaces;

namespace Notification.Api.Data;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _users;

    public UserRepository(IConfiguration configuration)
    {
        var client = new MongoClient(configuration.GetConnectionString("MongoDb"));
        var database = client.GetDatabase(configuration["DatabaseSettings:UserDbName"]);
        _users = database.GetCollection<User>("Users");
    }
    
    
    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _users.InsertOneAsync(user,null,  cancellationToken);
    }

    public async Task<User> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _users.Find(x => x.UserId == id).FirstOrDefaultAsync(cancellationToken);
    }
}