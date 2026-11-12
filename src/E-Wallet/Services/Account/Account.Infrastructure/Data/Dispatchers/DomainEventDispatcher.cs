using Account.Application.Interfaces;
using Account.Domain.Abstractions;
using MediatR;

namespace Account.Infrastructure.Data.Dispatchers;

public sealed class DomainEventDispatcher(AccountDbContext dbContext, IPublisher publisher) : IDomainEventDispatcher
{
    public async Task DispatchAsync(CancellationToken cancellationToken = default)
    {
        var aggregates = dbContext.ChangeTracker
            .Entries<IAggregate>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();
        var domainEvents = aggregates
            .SelectMany(e => e.DomainEvents)
            .ToList();
        aggregates.ForEach(e => e.ClearDomainEvents());
        foreach(var domainEvent in domainEvents)
        {
            await publisher.Publish(domainEvent, cancellationToken);
        }
    }
}