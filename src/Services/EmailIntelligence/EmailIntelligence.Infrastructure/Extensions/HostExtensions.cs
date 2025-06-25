using EmailIntelligence.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EmailIntelligence.Infrastructure.Extensions;

public static class HostExtensions
{
    public static async Task<IHost> InitializeDatabaseAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<EmailIntelligenceDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<EmailIntelligenceDbContext>>();

        try
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Apply seed data
            await EmailIntelligenceDbContextSeed.SeedAsync(context, logger);

            logger.LogInformation("Database initialization completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }

        return host;
    }
}
