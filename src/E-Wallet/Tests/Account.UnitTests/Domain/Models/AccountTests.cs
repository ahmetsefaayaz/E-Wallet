using Account.Domain.Exceptions;
using Account.Domain.Models;
using Account.Domain.ValueObjects;
using FluentAssertions;

namespace Account.UnitTests.Domain.Models;

public class AccountTests
{
    [Fact]
    public void Account_WhenGivenValidInstances_ShouldCreateAccountWithInitialBalance()
    {
        var userId = UserId.Of(Guid.NewGuid());
        var initialBalance = 1000m;

        var account = new Account.Domain.Models.Account(userId, initialBalance);

        // Assert
        account.UserId.Should().Be(userId);
        account.AccountBalance.Should().Be(initialBalance);
    }
    [Fact]
    public void CreateTransaction_WhenTransactionTypeIsWithdrawal_ShouldDecreaseAccountBalance()
    {
        var userId = UserId.Of(Guid.NewGuid());
        var account = new Account.Domain.Models.Account(userId, 1000m);
        var cardId = CardId.Of(Guid.NewGuid());
        var transactionType = TransactionType.Withdrawal;
        var amount = 500m;

        var transaction = account.CreateTransaction(cardId, transactionType, amount);

        // Assert
        transaction.Should().NotBeNull();
        account.Transactions.Should().Contain(transaction);
        account.AccountBalance.Should().Be(500m);
        transaction.TransactionType.Should().Be(transactionType);
        transaction.Amount.Should().Be(amount);
    }

    [Fact]
    public void CreateTransaction_WhenTransactionTypeIsDeposit_ShouldIncreaseAccountBalance()
    {
        var userId = UserId.Of(Guid.NewGuid());
        var account = new Account.Domain.Models.Account(userId, 1000m);
        var cardId = CardId.Of(Guid.NewGuid());
        var transactionType = TransactionType.Deposit;
        var amount = 500m;

        var transaction = account.CreateTransaction(cardId, transactionType, amount);

        // Assert
        transaction.TransactionType.Should().Be(transactionType);
        account.AccountBalance.Should().Be(1500m);
        transaction.Amount.Should().Be(amount);
        account.Transactions.Should().Contain(transaction);
    }
    [Fact]
    public void CreateTransaction_WhenTransactionTypeIsOutgoing_ShouldDecreaseAccountBalance()
    {
        var userId = UserId.Of(Guid.NewGuid());
        var account = new Account.Domain.Models.Account(userId, 1000m);
        var cardId = CardId.Of(Guid.NewGuid());
        var transactionType = TransactionType.OutgoingTransfer;
        var amount = 300m;
        //Act
        var transaction = account.CreateTransaction(cardId, transactionType, amount);
        //Assert
        transaction.Should().NotBeNull();
        account.AccountBalance.Should().Be(700m);
        transaction.TransactionType.Should().Be(transactionType);
        transaction.Amount.Should().Be(amount);
        account.Transactions.Should().Contain(transaction);
    }

    [Fact]
    public void CreateTransaction_WhenTransactionTypeIsIncoming_ShouldIncreaseAccountBalance()
    {
        //Arrange
        var userId = UserId.Of(Guid.NewGuid());
        var account = new Account.Domain.Models.Account(userId, 1000m);
        var cardId = CardId.Of(Guid.NewGuid());
        var transactionType = TransactionType.IncomingTransfer;
        var amount = 300m;
        //Act
        var transaction = account.CreateTransaction(cardId, transactionType, amount);
        //Assert
        transaction.Should().NotBeNull();
        account.AccountBalance.Should().Be(1300m);
        transaction.TransactionType.Should().Be(transactionType);
        transaction.Amount.Should().Be(amount);
        account.Transactions.Should().Contain(transaction);
    }
    [Theory]
    [InlineData(TransactionType.Withdrawal)]
    [InlineData(TransactionType.OutgoingTransfer)]
    public void CreateTransaction_WhenAmountExceedsBalance_ShouldThrowDomainException(TransactionType transactionType)
    {
        // Arrange
        var account = new Account.Domain.Models.Account(UserId.Of(Guid.NewGuid()), 500m);
        // Act
        Action act = () => account.CreateTransaction(CardId.Of(Guid.NewGuid()), transactionType, 600m);
        // Assert
        act.Should().Throw<DomainException>();
    }
    [Theory]
    [InlineData(-1)]
    [InlineData(-500)]
    [InlineData(1_000_001)]
    public void CreateTransaction_WhenAmountIsOutOfRange_ShouldThrowDomainException(decimal invalidAmount)
    {
        // Arrange
        var account = new Account.Domain.Models.Account(UserId.Of(Guid.NewGuid()), 5000m);
        // Act
        Action act = () => account.CreateTransaction(CardId.Of(Guid.NewGuid()), TransactionType.Deposit, invalidAmount);
        // Assert
        act.Should().Throw<DomainException>();
    }
    
    [Fact]
    public void CreateCard_WhenGivenValidCard_ShouldAddCardToAccount()
    {
        // Arrange
        var userId = UserId.Of(Guid.NewGuid());
        var account = new Account.Domain.Models.Account(userId, 1000m);
        
        //Act
        var card = account.CreateCard(CardHolderName.Of("Sssss"), CardNumber.Of("1234-5678-9012-3456"), CardExpiryDate.Of("12/25"), CardSecurityNumber.Of("123"));
        
        // Assert
        card.Should().NotBeNull();
        account.Cards.Should().Contain(card);
        card.CardHolderName.Should().Be(CardHolderName.Of("Sssss"));
    }
    [Fact]
    public void CreateCard_WhenAccountAlreadyHasThreeCards_ShouldThrowDomainException()
    {
        // Arrange
        var account = new Account.Domain.Models.Account(UserId.Of(Guid.NewGuid()), 1000m);
        for (int i = 0; i < 3; i++)
        {
            account.CreateCard(CardHolderName.Of("Test"), CardNumber.Of("1234-5678-9012-3456"), CardExpiryDate.Of("12/25"), CardSecurityNumber.Of("123"));
        }
        // Act
        Action act = () => account.CreateCard(CardHolderName.Of("Test 4"), CardNumber.Of("1234-5678-9012-3456"), CardExpiryDate.Of("12/25"), CardSecurityNumber.Of("123"));
        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("maximum 3 cards supported");
    }
    [Fact]
    public void UpdateCard_WhenGivenValidCard_ShouldUpdateCardInAccount()
    {
        // Arrange
        var userId = UserId.Of(Guid.NewGuid());
        var account = new Account.Domain.Models.Account(userId, 1000m);
        var card = account.CreateCard(CardHolderName.Of("Sssss"), CardNumber.Of("1234-5678-9012-3456"), CardExpiryDate.Of("12/25"), CardSecurityNumber.Of("123"));
        
        //Act
        account.UpdateCard(card.Id, CardHolderName.Of("Updated Name"), CardNumber.Of("9876-5432-1098-7654"), CardExpiryDate.Of("01/30"), CardSecurityNumber.Of("456"));
        
        // Assert
        var updatedCard = account.Cards.FirstOrDefault(c => c.Id == card.Id);
        updatedCard.Should().NotBeNull();
        updatedCard.CardHolderName.Should().Be(CardHolderName.Of("Updated Name"));
        updatedCard.CardExpiryDate.Should().Be(CardExpiryDate.Of("01/30"));
        updatedCard.CardSecurityNumber.Should().Be(CardSecurityNumber.Of("456"));
        updatedCard.CardNumber.Should().Be(CardNumber.Of("9876-5432-1098-7654"));
    }
    [Fact] 
    public void UpdateCard_WhenGivenInvalidCardId_ShouldThrowDomainException()
    {
        // Arrange
        var userId = UserId.Of(Guid.NewGuid());
        var account = new Account.Domain.Models.Account(userId, 1000m);
        
        //Act
        Action act = () => account.UpdateCard(CardId.Of(Guid.NewGuid()), CardHolderName.Of("Updated Name"), CardNumber.Of("9876-5432-1098-7654"), CardExpiryDate.Of("01/30"), CardSecurityNumber.Of("456"));
        
        // Assert
        act.Should().Throw<DomainException>();
    }
    [Fact]
    public void RemoveCard_WhenGivenValidCardId_ShouldRemoveCardFromAccount()
    {
        // Arrange
        var userId = UserId.Of(Guid.NewGuid());
        var account = new Account.Domain.Models.Account(userId, 1000m);
        var card = account.CreateCard(CardHolderName.Of("Sssss"), CardNumber.Of("1234-5678-9012-3456"), CardExpiryDate.Of("12/25"), CardSecurityNumber.Of("123"));
        
        //Act
        account.RemoveCard(card.Id);
        
        // Assert
        account.Cards.Should().NotContain(card);
    }

    [Fact]
    public void RemoveCard_WhenGivenInvalidCardId_ShouldThrowDomainException()
    {
        // Arrange
        var userId = UserId.Of(Guid.NewGuid());
        var account = new Account.Domain.Models.Account(userId, 1000m);
        
        //Act
        Action act = () => account.RemoveCard(CardId.Of(Guid.NewGuid()));
        
        // Assert
        act.Should().Throw<DomainException>();
    }
    
}