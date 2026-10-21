using Account.Domain.Abstractions;
using Account.Domain.Models;

namespace Account.Domain.Events;

public record TransactionCreatedEvent(Transaction Transaction): IDomainEvent;