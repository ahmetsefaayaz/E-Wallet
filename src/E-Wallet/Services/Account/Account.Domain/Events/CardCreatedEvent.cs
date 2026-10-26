using Account.Domain.Abstractions;
using Account.Domain.Models;
using Account.Domain.ValueObjects;

namespace Account.Domain.Events;

public record CardCreatedEvent(UserId UserId, Card Card): IDomainEvent;