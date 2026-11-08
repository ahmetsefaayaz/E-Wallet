namespace EWallet.Client.Models.Accounts;

public sealed class Card
{
    public Guid Id { get; set; }

    public Guid AccountId { get; set; }

    public string? CardHolderName { get; set; }

    public string? CardNumber { get; set; }

    public string? CardExpiryDate { get; set; }

    public string? CardSecurityNumber { get; set; }
}

public sealed class GetCardsByAccountIdResponse
{
    public List<Card> Cards { get; set; } = [];
}

public sealed class CreateCardRequest
{
    public CardInput Card { get; set; } = new();
}

public sealed class CreateCardResponse
{
    public Guid CardId { get; set; }
}

public sealed class UpdateCardRequest
{
    public CardInput Card { get; set; } = new();
}

public sealed class UpdateCardResponse
{
    public bool IsSuccess { get; set; }
}

public sealed class DeleteCardResponse
{
    public bool IsSuccess { get; set; }
}

public sealed class CardInput
{
    public string CardHolderName { get; set; } = string.Empty;

    public string CardNumber { get; set; } = string.Empty;

    public string CardExpiryDate { get; set; } = string.Empty;

    public string CardSecurityNumber { get; set; } = string.Empty;
}
