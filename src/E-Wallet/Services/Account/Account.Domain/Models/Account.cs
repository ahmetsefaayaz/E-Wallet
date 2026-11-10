using Account.Domain.Abstractions;
using Account.Domain.Events;
using Account.Domain.Exceptions;
using Account.Domain.ValueObjects;

namespace Account.Domain.Models;

public class Account : Aggregate<AccountId>
{
    private readonly List<Transaction> _transactions = new();
    private readonly List<Card> _cards = new();
    public IReadOnlyList<Card> Cards => _cards.AsReadOnly();
    public IReadOnlyList<Transaction> Transactions => _transactions.AsReadOnly();
    public UserId UserId { get; init; }
    public decimal AccountBalance { get; private set; }

    private Account() {}

    public Account(UserId userId, decimal accountBalance = 0)
    {
        UserId = userId;
        AccountBalance = accountBalance;
        Id = AccountId.Of(Guid.NewGuid());
    }
    
    private void ValidateAmount(decimal amount)
    {
        if(amount < 0) throw new DomainException("Amount cannot be negative");
        if (amount > 1000000) throw new DomainException("Amount cannot be greater than 1000000");
    }
    public Transaction CreateTransaction(CardId? cardId, TransactionType transactionType, decimal amount)
    {
        if (transactionType == TransactionType.Withdrawal || transactionType == TransactionType.OutgoingTransfer)
        {
            DecreaseAccountBalance(amount);
        }
        else if (transactionType == TransactionType.IncomingTransfer || transactionType == TransactionType.Deposit)
        {
            IncreaseAccountBalance(amount);
        }
        else throw new DomainException("Invalid transaction type");
        var transaction = new Transaction
        (
            Id,
            cardId,
            transactionType,
            Status.Pending,
            amount
        );
        _transactions.Add(transaction);
        AddDomainEvent(new TransactionCreatedEvent(UserId, transaction));
        return transaction;
    }

    private void IncreaseAccountBalance(decimal amount)
    {
        ValidateAmount(amount);
        AccountBalance  += amount;
    }
    private void DecreaseAccountBalance(decimal amount)
    {
        ValidateAmount(amount);
        if(amount > AccountBalance) throw new DomainException("Amount cannot be greater than AccountBalance");
        AccountBalance -= amount;
    }
    public Card CreateCard(CardHolderName cardHolderName,
        CardNumber cardNumber, CardExpiryDate cardExpiryDate, CardSecurityNumber cardSecurityNumber)
    {
        if(_cards.Count >= 3) throw new DomainException("maximum 3 cards supported");
        var card = new Card(Id, cardHolderName, cardNumber, cardExpiryDate, cardSecurityNumber);
        _cards.Add(card);
        AddDomainEvent(new CardCreatedEvent(UserId, card));
        return card;
    }
    
    public void UpdateCard(CardId cardId, CardHolderName cardHolderName,
        CardNumber cardNumber, CardExpiryDate cardExpiryDate, CardSecurityNumber cardSecurityNumber)
    {
        var card = _cards.FirstOrDefault(c => c.Id == cardId);
        if (card == null) throw new DomainException("Card not found");
        card.UpdateCard(cardHolderName, cardNumber, cardExpiryDate, cardSecurityNumber);
    }
    public void RemoveCard(CardId cardId)
    {
        var card = _cards.FirstOrDefault(c => c.Id == cardId);
        if (card == null) throw new DomainException("Card not found");
        _cards.Remove(card);
    }
    
    
}