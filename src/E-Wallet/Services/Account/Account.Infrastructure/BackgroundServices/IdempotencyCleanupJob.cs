using Account.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Account.Infrastructure.BackgroundServices;

public class IdempotencyCleanupJob(IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
            var yesterday = DateTime.UtcNow.AddDays(-1);

            await dbContext.IdempotencyRecords
                .Where(x => x.CreatedAt < yesterday)
                .ExecuteDeleteAsync(stoppingToken);
            
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}