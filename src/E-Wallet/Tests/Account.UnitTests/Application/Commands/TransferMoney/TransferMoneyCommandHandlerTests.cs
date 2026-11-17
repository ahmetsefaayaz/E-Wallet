using Account.Application.Account.Commands.TransferMoney;
using Account.Application.Exceptions;
using Account.Application.Interfaces;
using Account.Domain.Exceptions;
using Account.Domain.ValueObjects;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;

namespace Account.UnitTests.Application.Commands.TransferMoney;

public class TransferMoneyCommandHandlerTests
{
    private readonly Mock<IAccountDbContext> _dbContextMock;
    private readonly Mock<IDomainEventDispatcher> _domainEventDispatcherMock;
    private readonly TransferMoneyCommandHandler _handler;

    public TransferMoneyCommandHandlerTests()
    {
        _dbContextMock = new Mock<IAccountDbContext>();
        _domainEventDispatcherMock = new Mock<IDomainEventDispatcher>();
        _handler = new TransferMoneyCommandHandler(_dbContextMock.Object, _domainEventDispatcherMock.Object);
    }

    [Fact]
    public async Task Handle_WhenAccountsAndCardAreValid_ShouldTransferMoneyAndSaveChanges()
    {
        // Arrange
        var senderUserId = Guid.NewGuid();
        var receiverUserId = Guid.NewGuid();

        var senderAccount = new Account.Domain.Models.Account(UserId.Of(senderUserId), 1000m);
        var senderCard = senderAccount.CreateCard(
            CardHolderName.Of("Sender"),
            CardNumber.Of("1234-5678-9012-3456"),
            CardExpiryDate.Of("12/28"),
            CardSecurityNumber.Of("123"));

        var receiverAccount = new Account.Domain.Models.Account(UserId.Of(receiverUserId), 400m);

        var accounts = new List<Account.Domain.Models.Account> { senderAccount, receiverAccount };
        var mockDbSet = accounts.BuildMockDbSet();
        _dbContextMock.Setup(x => x.Accounts).Returns(mockDbSet.Object);

        var command = new TransferMoneyCommand(
            AccountId: senderAccount.Id.Value,
            ReceiverAccountId: receiverAccount.Id.Value,
            CardId: senderCard.Id.Value,
            CurrentUserId: senderUserId,
            Amount: 300m,
            IdempotencyKey: "transfer-key-123");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        senderAccount.AccountBalance.Should().Be(700m);
        receiverAccount.AccountBalance.Should().Be(700m);

        _domainEventDispatcherMock.Verify(x => x.DispatchAsync(It.IsAny<CancellationToken>()), Times.Once);
        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenSenderAccountDoesNotExist_ShouldThrowAccountNotFoundException()
    {
        // Arrange
        var accounts = new List<Account.Domain.Models.Account>();
        var mockDbSet = accounts.BuildMockDbSet();
        _dbContextMock.Setup(x => x.Accounts).Returns(mockDbSet.Object);

        var command = new TransferMoneyCommand(
            AccountId: Guid.NewGuid(),
            ReceiverAccountId: Guid.NewGuid(),
            CardId: Guid.NewGuid(),
            CurrentUserId: Guid.NewGuid(),
            Amount: 300m,
            IdempotencyKey: "transfer-key-123");

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AccountNotFoundException>();
        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenSenderCardDoesNotExist_ShouldThrowCardNotFoundException()
    {
        // Arrange
        var senderUserId = Guid.NewGuid();
        var senderAccount = new Account.Domain.Models.Account(UserId.Of(senderUserId), 1000m);

        var accounts = new List<Account.Domain.Models.Account> { senderAccount };
        var mockDbSet = accounts.BuildMockDbSet();
        _dbContextMock.Setup(x => x.Accounts).Returns(mockDbSet.Object);

        var command = new TransferMoneyCommand(
            AccountId: senderAccount.Id.Value,
            ReceiverAccountId: Guid.NewGuid(),
            CardId: Guid.NewGuid(),
            CurrentUserId: senderUserId,
            Amount: 300m,
            IdempotencyKey: "transfer-key-123");

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<CardNotFoundException>();
        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenReceiverAccountDoesNotExist_ShouldThrowAccountNotFoundException()
    {
        // Arrange
        var senderUserId = Guid.NewGuid();
        var senderAccount = new Account.Domain.Models.Account(UserId.Of(senderUserId), 1000m);
        var senderCard = senderAccount.CreateCard(
            CardHolderName.Of("Sender"),
            CardNumber.Of("1234-5678-9012-3456"),
            CardExpiryDate.Of("12/28"),
            CardSecurityNumber.Of("123"));

        var accounts = new List<Account.Domain.Models.Account> { senderAccount };
        var mockDbSet = accounts.BuildMockDbSet();
        _dbContextMock.Setup(x => x.Accounts).Returns(mockDbSet.Object);

        var command = new TransferMoneyCommand(
            AccountId: senderAccount.Id.Value,
            ReceiverAccountId: Guid.NewGuid(),
            CardId: senderCard.Id.Value,
            CurrentUserId: senderUserId,
            Amount: 300m,
            IdempotencyKey: "transfer-key-123");

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AccountNotFoundException>();
        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenSenderBalanceIsInsufficient_ShouldThrowDomainException()
    {
        // Arrange
        var senderUserId = Guid.NewGuid();
        var senderAccount = new Account.Domain.Models.Account(UserId.Of(senderUserId), 100m);
        var senderCard = senderAccount.CreateCard(
            CardHolderName.Of("Sender"),
            CardNumber.Of("1234-5678-9012-3456"),
            CardExpiryDate.Of("12/28"),
            CardSecurityNumber.Of("123"));

        var receiverAccount = new Account.Domain.Models.Account(UserId.Of(Guid.NewGuid()), 400m);

        var accounts = new List<Account.Domain.Models.Account> { senderAccount, receiverAccount };
        var mockDbSet = accounts.BuildMockDbSet();
        _dbContextMock.Setup(x => x.Accounts).Returns(mockDbSet.Object);

        var command = new TransferMoneyCommand(
            AccountId: senderAccount.Id.Value,
            ReceiverAccountId: receiverAccount.Id.Value,
            CardId: senderCard.Id.Value,
            CurrentUserId: senderUserId,
            Amount: 500m,
            IdempotencyKey: "transfer-key-123");

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Amount cannot be greater than AccountBalance");
        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
