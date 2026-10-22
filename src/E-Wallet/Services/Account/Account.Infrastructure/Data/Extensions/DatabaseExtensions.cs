using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Account.Infrastructure.Data.Extensions;

public static class DatabaseExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
        context.Database.MigrateAsync().GetAwaiter().GetResult();
        await SeedAsync(context);
    }

    private static async Task SeedAsync(AccountDbContext context)
    {
        if (context.Accounts.Any())
        {
            return;
        }
        var accounts = await SeedAccountsAsync(context);
        await SeedCardsAsync(context,  accounts);
        await SeedTransactionsAsync(context, accounts);
        await context.SaveChangesAsync();
    }

    private static async Task<IEnumerable<Domain.Models.Account>> SeedAccountsAsync(AccountDbContext context)
    {
        await context.Accounts.AddRangeAsync(SeedData.SeedAccounts);
        await context.SaveChangesAsync();
        return await context.Accounts.ToListAsync();
    }

    private static async Task SeedCardsAsync(AccountDbContext context, IEnumerable<Domain.Models.Account> accounts)
    {
        SeedData.CreateCardToAccount1(accounts.Where(a => a.AccountBalance == 500).FirstOrDefault());
        SeedData.CreateCardToAccount2(accounts.Where(a => a.AccountBalance == 0).FirstOrDefault());
    }

    private static async Task SeedTransactionsAsync(AccountDbContext context, IEnumerable<Domain.Models.Account> accounts)
    {
        SeedData.CreateTransactionToAccount1(accounts.FirstOrDefault(a => a.AccountBalance == 500)!);
        SeedData.CreateTransactionToAccount2(accounts.FirstOrDefault(a => a.AccountBalance == 0));
    }
}