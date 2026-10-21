using Account.Domain.Abstractions;
using Account.Domain.ValueObjects;

namespace Account.Domain.Models;

public class Card: Entity<CardId>
{
    internal Card(AccountId accountId, CardHolderName cardHolderName,
        CardNumber cardNumber, CardExpiryDate cardExpiryDate, CardSecurityNumber cardSecurityNumber)
    {
        Id = CardId.Of(Guid.NewGuid());
        AccountId = accountId;
        CardHolderName = cardHolderName;
        CardNumber = cardNumber;
        CardExpiryDate = cardExpiryDate;
        CardSecurityNumber = cardSecurityNumber;
    }

    internal void UpdateCard(CardHolderName cardHolderName,
        CardNumber cardNumber, CardExpiryDate cardExpiryDate, CardSecurityNumber cardSecurityNumber)
    {
        CardHolderName = cardHolderName;
        CardNumber = cardNumber;
        CardExpiryDate = cardExpiryDate;
        CardSecurityNumber = cardSecurityNumber;
    }
    public AccountId AccountId { get; private set; }
    public CardHolderName CardHolderName { get; private set; }
    public CardNumber CardNumber { get; private set; }
    public CardExpiryDate CardExpiryDate { get; private set; }
    public CardSecurityNumber CardSecurityNumber { get; private set; }
}