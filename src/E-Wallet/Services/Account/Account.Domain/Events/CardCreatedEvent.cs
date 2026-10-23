using Account.Domain.Abstractions;
using Account.Domain.Models;

namespace Account.Domain.Events;

public record CardCreatedEvent(Card Card): IDomainEvent;