using Account.Application.Dtos;
using Account.Domain.Models;

namespace Account.Application.Extensions;

public static class CardExtensions
{
    public static IEnumerable<GetCardDto> ToGetCardsDto(this IEnumerable<Card> cards) =>
        cards.Select(c => new GetCardDto
        (
            c.Id.Value,
            c.AccountId.Value,
            c.CardHolderName.Value,
            c.CardNumber.Value,
            c.CardExpiryDate.Value,
            c.CardSecurityNumber.Value
        ));
}