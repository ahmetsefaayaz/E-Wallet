namespace BuildingBlocks.Interfaces;

public interface IIdempotencyService
{
    Task<bool> RequestExistsAsync(Guid userId, string operation, string key, CancellationToken cancellationToken);
    
    void AddRequest(Guid userId, string operation, string key); 
}