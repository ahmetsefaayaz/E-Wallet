using Account.Domain.Abstractions;
using Account.Domain.Models;

namespace Account.Domain.Events;

public class CardCreatedEvent(Card Card): IDomainEvent;