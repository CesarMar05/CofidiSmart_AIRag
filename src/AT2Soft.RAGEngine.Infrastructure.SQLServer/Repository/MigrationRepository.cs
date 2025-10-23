using AT2Soft.RAGEngine.Application.Persistence.Migrations;
using AT2Soft.RAGEngine.Infrastructure.SQLServer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AT2Soft.RAGEngine.Infrastructure.SQLServer.Repository;

public class MigrationRepository(RAGSqlServerDbContext context, ILogger<MigrationRepository> logger) : IMigrationRepository
{
    private readonly RAGSqlServerDbContext _context = context;
    private readonly ILogger<MigrationRepository> _logger = logger;

    public async Task<string> ActualStatusAsync(CancellationToken cancellationToken = default)
    {
        var response = string.Empty;

        var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync(cancellationToken: cancellationToken);
        if (!appliedMigrations.Any())
        {
            response = $"The Database not contains Migrations.\n";
        }
        else
        {
            var lastAppliedMigration = appliedMigrations.Last();
            response += $"You're on schema version: {lastAppliedMigration}\n";

            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync(cancellationToken: cancellationToken);
            if (pendingMigrations.Any())
            {
                response += $"You have {pendingMigrations.Count()} pending migrations to apply.\n";
            }
        }

        return response;
    }

    public async Task<string> UpdateDatabaseAsync(CancellationToken cancellationToken = default)
    {
        string response;

        var pendingMigrations = await _context.Database.GetPendingMigrationsAsync(cancellationToken: cancellationToken);
        if (pendingMigrations.Any())
        {
            var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync(cancellationToken: cancellationToken);
            response = $"The Database was {(appliedMigrations.Any() ? "updating" : "creating")} successfully";
            await _context.Database.MigrateAsync(cancellationToken: cancellationToken);
        }
        else
        {
            response = $"The Database are updating";
        }

        return response;
    }
}
