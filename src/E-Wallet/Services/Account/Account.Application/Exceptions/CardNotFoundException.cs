using BuildingBlocks.Exceptions;

namespace Account.Application.Exceptions;

public class CardNotFoundException: NotFoundException
{
    public CardNotFoundException(string cardId) : base("Card", cardId)
    {
    }
}