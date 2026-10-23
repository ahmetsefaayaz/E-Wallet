
using Account.Domain.Models;
using Account.Domain.ValueObjects;

namespace Account.Infrastructure.Data.Extensions;

public static class SeedData
{
    public static IEnumerable<Domain.Models.Account> SeedAccounts=>
        new List<Domain.Models.Account>
        {
            new Domain.Models.Account(UserId.Of(Guid.NewGuid()), accountBalance: 500)
            {
                Id = AccountId.Of(Guid.NewGuid())
            },
            new Domain.Models.Account(UserId.Of(Guid.NewGuid()))
            {
                Id = AccountId.Of(Guid.NewGuid())
            }
        };

    public static IEnumerable<Card> CreateCardToAccount1(Domain.Models.Account account) =>
        new List<Card>()
        {
            account.CreateCard(CardHolderName.Of("Sefa"), CardNumber.Of("1234"), CardExpiryDate.Of("01/2028"),
                CardSecurityNumber.Of("123"))
        };

    public static IEnumerable<Card> CreateCardToAccount2(Domain.Models.Account account) =>
        new List<Card>()
        {
            account.CreateCard(CardHolderName.Of("Lara"), CardNumber.Of("5678"), CardExpiryDate.Of("01/2029"),
                CardSecurityNumber.Of("431"))
        };

    public static IEnumerable<Transaction> CreateTransactionToAccount1(Domain.Models.Account account) =>
        new List<Transaction>()
        {
            account.CreateTransaction(account.Cards.FirstOrDefault(a => a.CardHolderName.Value.Equals("Sefa"))!.Id, TransactionType.Withdrawal, 235),
            account.CreateTransaction(account.Cards.FirstOrDefault(a => a.CardHolderName.Value.Equals("Sefa"))!.Id, TransactionType.Withdrawal, 130)
        };
    
    public static IEnumerable<Transaction> CreateTransactionToAccount2(Domain.Models.Account account) =>
        new List<Transaction>()
        {
            account.CreateTransaction(account.Cards.FirstOrDefault(a => a.CardHolderName.Value.Equals("Lara"))!.Id, TransactionType.Deposit, 210)
        };
}