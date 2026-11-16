using Account.Application.Interfaces;
using Account.Domain.Models;
using Account.Domain.ValueObjects;
using BuildingBlocks.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Account.Application.Idempotency;

internal sealed class IdempotencyService(IAccountDbContext dbContext): IIdempotencyService
{
    public async Task<bool> RequestExistsAsync(Guid userId, string operation, string key, CancellationToken cancellationToken)
    {
        return await dbContext.IdempotencyRecords
            .AsNoTracking()
            .AnyAsync(x => x.UserId == UserId.Of(userId) && x.Operation == operation && x.Key == key, cancellationToken);
    }

    public void AddRequest(Guid userId, string operation, string key)
    {
        dbContext.IdempotencyRecords.Add(new IdempotencyRecord(UserId.Of(userId), operation, key));
    }
}