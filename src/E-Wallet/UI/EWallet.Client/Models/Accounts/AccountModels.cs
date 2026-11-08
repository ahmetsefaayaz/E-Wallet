namespace EWallet.Client.Models.Accounts;

public sealed class GetAccountsResponse
{
    public PaginatedAccounts? PaginatedResult { get; set; }
}

public sealed class PaginatedAccounts
{
    public List<Account> Data { get; set; } = [];

    public int PageIndex { get; set; }

    public int PageSize { get; set; }

    public long TotalItemCount { get; set; }
}

public sealed class Account
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public double AccountBalance { get; set; }
}

public sealed class CreateAccountResponse
{
    public Guid AccountId { get; set; }
}

public sealed class GetAccountByIdResponse
{
    public Account? Account { get; set; }
}

public sealed class DeleteAccountResponse
{
    public bool IsSuccess { get; set; }
}
