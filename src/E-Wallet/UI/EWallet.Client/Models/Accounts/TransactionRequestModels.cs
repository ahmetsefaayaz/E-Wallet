namespace EWallet.Client.Models.Accounts;

public sealed class DepositRequest
{
    public double Amount { get; set; }
}

public sealed class DepositResponse
{
    public bool IsSuccess { get; set; }
}

public sealed class WithdrawRequest
{
    public double Amount { get; set; }
}

public sealed class WithdrawResponse
{
    public bool IsSuccess { get; set; }
}

public sealed class TransferRequest
{
    public Guid ReceiverId { get; set; }

    public double Amount { get; set; }
}
