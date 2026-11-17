using Account.Application.Account.Commands.CreateCard;
using Account.Application.Dtos;
using Account.Application.Exceptions;
using Account.Application.Interfaces;
using Account.Domain.Exceptions;
using Account.Domain.ValueObjects;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;

namespace Account.UnitTests.Application.Commands.CreateCard;

public class CreateCardCommandHandlerTests
{
    private readonly Mock<IAccountDbContext> _dbContextMock;
    private readonly Mock<IDomainEventDispatcher> _domainEventDispatcherMock;
    private readonly CreateCardCommandHandler _handler;

    public CreateCardCommandHandlerTests()
    {
        _dbContextMock = new Mock<IAccountDbContext>();
        _domainEventDispatcherMock = new Mock<IDomainEventDispatcher>();
        _handler = new CreateCardCommandHandler(_dbContextMock.Object, _domainEventDispatcherMock.Object);
    }

    [Fact]
    public async Task Handle_WhenAccountExistsAndUnderCardLimit_ShouldCreateCardAndReturnCardId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fakeAccount = new Account.Domain.Models.Account(UserId.Of(userId), 1000m);
        var accounts = new List<Account.Domain.Models.Account> { fakeAccount };

        var mockDbSet = accounts.BuildMockDbSet();
        _dbContextMock.Setup(x => x.Accounts).Returns(mockDbSet.Object);

        var cardDto = new CreateCardDto("Ayaz", "1234-5678-9012-3456", "12/28", "123");
        var command = new CreateCardCommand(fakeAccount.Id.Value, userId, cardDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.CardId.Should().NotBeEmpty();
        fakeAccount.Cards.Should().ContainSingle(c => c.Id.Value == result.CardId);

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

        var cardDto = new CreateCardDto("Ayaz", "1234-5678-9012-3456", "12/28", "123");
        var command = new CreateCardCommand(Guid.NewGuid(), Guid.NewGuid(), cardDto);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AccountNotFoundException>();
        _domainEventDispatcherMock.Verify(x => x.DispatchAsync(It.IsAny<CancellationToken>()), Times.Never);
        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenAccountAlreadyHasThreeCards_ShouldThrowDomainException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fakeAccount = new Account.Domain.Models.Account(UserId.Of(userId), 1000m);
        for (var i = 0; i < 3; i++)
        {
            fakeAccount.CreateCard(
                CardHolderName.Of("Ayaz"),
                CardNumber.Of("1234-5678-9012-3456"),
                CardExpiryDate.Of("12/28"),
                CardSecurityNumber.Of("123"));
        }

        var accounts = new List<Account.Domain.Models.Account> { fakeAccount };
        var mockDbSet = accounts.BuildMockDbSet();
        _dbContextMock.Setup(x => x.Accounts).Returns(mockDbSet.Object);

        var cardDto = new CreateCardDto("Ayaz", "9999-8888-7777-6666", "12/29", "456");
        var command = new CreateCardCommand(fakeAccount.Id.Value, userId, cardDto);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("maximum 3 cards supported");
        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
