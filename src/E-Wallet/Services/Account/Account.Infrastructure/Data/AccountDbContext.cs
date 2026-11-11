using System.Reflection;
using Account.Application.Interfaces;
using Account.Domain.Abstractions;
using Account.Domain.Models;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Account.Infrastructure.Data;

public class AccountDbContext: DbContext, IAccountDbContext
{
    private readonly IPublisher _publisher;
    public AccountDbContext(DbContextOptions<AccountDbContext> options, IPublisher publisher): base(options) 
    {
        _publisher = publisher;
    }
    public DbSet<IdempotencyRecord> IdempotencyRecords { get; set; }
    public DbSet<Domain.Models.Account> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var aggregates = ChangeTracker
            .Entries<IAggregate>()
            .Where(a => a.Entity.DomainEvents.Any())
            .Select(a => a.Entity)
            .ToList();

        var domainEvents = aggregates
            .SelectMany(a => a.DomainEvents)
            .ToList();
        
        aggregates.ForEach(a => a.ClearDomainEvents());
        
        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }
        
        return await base.SaveChangesAsync(cancellationToken);
    }
    
}