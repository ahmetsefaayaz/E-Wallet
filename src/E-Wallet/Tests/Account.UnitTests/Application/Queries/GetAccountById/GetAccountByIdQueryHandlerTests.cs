using Account.Application.Account.Queries.GetAccountById;
using Account.Application.Exceptions;
using Account.Application.Interfaces;
using Account.Domain.ValueObjects;
using BuildingBlocks.Exceptions;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;

namespace Account.UnitTests.Application.Queries.GetAccountById;

public class GetAccountByIdQueryHandlerTests
{
    private readonly Mock<IAccountDbContext> _dbContextMock;
    private readonly GetAccountByIdQueryHandler _handler;

    public GetAccountByIdQueryHandlerTests()
    {
        _dbContextMock = new Mock<IAccountDbContext>();
        _handler = new GetAccountByIdQueryHandler(_dbContextMock.Object);
    }

    [Fact]
    public async Task Handle_WhenAccountExistsAndUserIsOwner_ShouldReturnAccountDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fakeAccount = new Account.Domain.Models.Account(UserId.Of(userId), 1500m);
        var accounts = new List<Account.Domain.Models.Account> { fakeAccount };

        var mockDbSet = accounts.BuildMockDbSet();
        _dbContextMock.Setup(x => x.Accounts).Returns(mockDbSet.Object);

        var query = new GetAccountByIdQuery(
            AccountId: fakeAccount.Id.Value,
            CurrentUserId: userId,
            IsAccountDepartment: false);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Account.Should().NotBeNull();
        result.Account.Id.Should().Be(fakeAccount.Id.Value);
        result.Account.UserId.Should().Be(userId);
        result.Account.AccountBalance.Should().Be(1500m);
    }

    [Fact]
    public async Task Handle_WhenUserIsNotOwnerButIsAccountDepartment_ShouldReturnAccountDto()
    {
        // Arrange
        var ownerUserId = Guid.NewGuid();
        var departmentUserId = Guid.NewGuid();
        var fakeAccount = new Account.Domain.Models.Account(UserId.Of(ownerUserId), 2500m);
        var accounts = new List<Account.Domain.Models.Account> { fakeAccount };

        var mockDbSet = accounts.BuildMockDbSet();
        _dbContextMock.Setup(x => x.Accounts).Returns(mockDbSet.Object);

        var query = new GetAccountByIdQuery(
            AccountId: fakeAccount.Id.Value,
            CurrentUserId: departmentUserId,
            IsAccountDepartment: true);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Account.Id.Should().Be(fakeAccount.Id.Value);
        result.Account.AccountBalance.Should().Be(2500m);
    }

    [Fact]
    public async Task Handle_WhenAccountDoesNotExist_ShouldThrowAccountNotFoundException()
    {
        // Arrange
        var accounts = new List<Account.Domain.Models.Account>();
        var mockDbSet = accounts.BuildMockDbSet();
        _dbContextMock.Setup(x => x.Accounts).Returns(mockDbSet.Object);

        var query = new GetAccountByIdQuery(
            AccountId: Guid.NewGuid(),
            CurrentUserId: Guid.NewGuid(),
            IsAccountDepartment: false);

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AccountNotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenUserIsNotOwnerAndNotAccountDepartment_ShouldThrowForbiddenAccessException()
    {
        // Arrange
        var ownerUserId = Guid.NewGuid();
        var unauthorizedUserId = Guid.NewGuid();
        var fakeAccount = new Account.Domain.Models.Account(UserId.Of(ownerUserId), 1000m);
        var accounts = new List<Account.Domain.Models.Account> { fakeAccount };

        var mockDbSet = accounts.BuildMockDbSet();
        _dbContextMock.Setup(x => x.Accounts).Returns(mockDbSet.Object);

        var query = new GetAccountByIdQuery(
            AccountId: fakeAccount.Id.Value,
            CurrentUserId: unauthorizedUserId,
            IsAccountDepartment: false);

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>()
            .WithMessage("You are not authorized to view this account.");
    }
}
