namespace EWallet.Client.Models.Accounts;

public sealed class GetTransactionsByAccountIdResponse
{
    public List<Transaction> Transactions { get; set; } = [];
}

public sealed class Transaction
{
    public Guid Id { get; set; }

    public Guid AccountId { get; set; }

    public Guid? CardId { get; set; }

    public int TransactionType { get; set; }

    public int Status { get; set; }

    public double Amount { get; set; }
}
