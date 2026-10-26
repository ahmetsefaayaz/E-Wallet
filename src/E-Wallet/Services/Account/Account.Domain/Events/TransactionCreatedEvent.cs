using Account.Domain.Abstractions;
using Account.Domain.Models;
using Account.Domain.ValueObjects;

namespace Account.Domain.Events;

public record TransactionCreatedEvent(UserId UserId, Transaction Transaction): IDomainEvent;