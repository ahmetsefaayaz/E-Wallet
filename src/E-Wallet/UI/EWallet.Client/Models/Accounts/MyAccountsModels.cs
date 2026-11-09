namespace EWallet.Client.Models.Accounts;

public sealed class GetMyAccountsResponse
{
    public List<Account> Accounts { get; set; } = [];
}
