using Account.Application.Account.Commands.DepositTransaction;
using Account.Application.Exceptions;
using Account.Application.Interfaces;
using Account.Domain.ValueObjects;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;

namespace Account.UnitTests.Application.Commands.DepositMoney;

public class DepositTransactionCommandHandlerTests
{
    private readonly Mock<IAccountDbContext> _dbContextMock;
    private readonly Mock<IDomainEventDispatcher> _domainEventDispatcherMock;
    private readonly DepositTransactionCommandHandler _handler;

    public DepositTransactionCommandHandlerTests()
    {
        _dbContextMock = new Mock<IAccountDbContext>();
        _domainEventDispatcherMock = new Mock<IDomainEventDispatcher>();
        _handler = new DepositTransactionCommandHandler(_dbContextMock.Object, _domainEventDispatcherMock.Object);
    }

    [Fact]
    public async Task Handle_WhenAccountAndCardExist_ShouldIncreaseBalanceAndSaveChanges()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fakeAccount = new Account.Domain.Models.Account(UserId.Of(userId), 1000m);
        var card = fakeAccount.CreateCard(
            CardHolderName.Of("Ayaz"),
            CardNumber.Of("1234-5678-9012-3456"),
            CardExpiryDate.Of("12/28"),
            CardSecurityNumber.Of("123"));

        var accounts = new List<Account.Domain.Models.Account> { fakeAccount };
        var mockDbSet = accounts.BuildMockDbSet();
        _dbContextMock.Setup(x => x.Accounts).Returns(mockDbSet.Object);
        
        var command = new DepositTransactionCommand(
            AccountId: fakeAccount.Id.Value,
            CardId: card.Id.Value,
            CurrentUserId: userId,
            Amount: 500m,
            IdempotencyKey: "idemp-key-123");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        fakeAccount.AccountBalance.Should().Be(1500m);
        fakeAccount.Transactions.Should().ContainSingle(t =>
            t.Amount == 500m && t.TransactionType == TransactionType.Deposit);

        _domainEventDispatcherMock.Verify(x => x.DispatchAsync(It.IsAny<CancellationToken>()), Times.Once);
        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAccountDoesNotExist_ShouldThrowAccountNotFoundException()
    {
        // Arrange
        var accounts = new List<Account.Domain.Models.Account>();
        var mockDbSet = accounts.BuildMockDbSet();
        _dbContextMock.Setup(x => x.Accounts).Returns(mockDbSet.Object);

        var command = new DepositTransactionCommand(
            AccountId: Guid.NewGuid(),
            CardId: Guid.NewGuid(),
            CurrentUserId: Guid.NewGuid(),
            Amount: 500m,
            IdempotencyKey: "idemp-key-123");

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AccountNotFoundException>();
        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenCardDoesNotExistInAccount_ShouldThrowCardNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fakeAccount = new Account.Domain.Models.Account(UserId.Of(userId), 1000m);
        var accounts = new List<Account.Domain.Models.Account> { fakeAccount };

        var mockDbSet = accounts.BuildMockDbSet();
        _dbContextMock.Setup(x => x.Accounts).Returns(mockDbSet.Object);

        var command = new DepositTransactionCommand(
            AccountId: fakeAccount.Id.Value,
            CardId: Guid.NewGuid(),
            CurrentUserId: userId,
            Amount: 500m,
            IdempotencyKey: "idemp-key-123");

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<CardNotFoundException>();
        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
