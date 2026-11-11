using Account.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Account.Application.Interfaces;

public interface IAccountDbContext
{
    DbSet<IdempotencyRecord> IdempotencyRecords { get; }
    DbSet<Domain.Models.Account> Accounts { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}