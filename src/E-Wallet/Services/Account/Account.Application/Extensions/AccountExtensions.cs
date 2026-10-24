using Account.Application.Dtos;

namespace Account.Application.Extensions;

public static class AccountExtensions
{
    public static IEnumerable<GetAccountDto> ToGetAccountsDto(this IEnumerable<Domain.Models.Account> accounts)
    {
        return accounts.Select(a => new GetAccountDto
        (
            a.Id.Value,
            a.UserId.Value,
            a.AccountBalance,
            a.Cards.ToGetCardsDto(),
            a.Transactions.ToTransactionsDto()
        ));
        
    }

    public static GetAccountDto ToGetAccountDto(this Domain.Models.Account account)
    {
        return new GetAccountDto(account.Id.Value, account.UserId.Value, account.AccountBalance,
            account.Cards.ToGetCardsDto(), account.Transactions.ToTransactionsDto());
    }
}