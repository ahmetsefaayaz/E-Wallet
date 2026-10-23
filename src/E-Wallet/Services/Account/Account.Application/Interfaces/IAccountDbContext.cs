using Microsoft.EntityFrameworkCore;

namespace Account.Application.Interfaces;

public interface IAccountDbContext
{
    DbSet<Domain.Models.Account> Accounts { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}