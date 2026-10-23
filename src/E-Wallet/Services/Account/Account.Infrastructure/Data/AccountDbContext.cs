using System.Reflection;
using Account.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Account.Infrastructure.Data;

public class AccountDbContext: DbContext, IAccountDbContext
{
    public AccountDbContext(DbContextOptions<AccountDbContext> options): base(options) {}
    
    public DbSet<Domain.Models.Account> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
    
}