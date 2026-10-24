using Account.Application.Dtos;
using Account.Domain.Models;

namespace Account.Application.Extensions;

public static class TransactionExtensions
{
    public static IEnumerable<TransactionDto> ToTransactionsDto(this IEnumerable<Transaction> transactions) =>
        transactions.Select(t => new TransactionDto(
            t.Id.Value,
            t.AccountId.Value,
            t.CardId?.Value,
            t.TransactionType,
            t.Status,
            t.Amount
        ));
}