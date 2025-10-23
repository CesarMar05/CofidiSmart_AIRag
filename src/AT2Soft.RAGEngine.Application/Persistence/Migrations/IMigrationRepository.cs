
namespace AT2Soft.RAGEngine.Application.Persistence.Migrations;

public interface IMigrationRepository
{
    Task<string> ActualStatusAsync(CancellationToken cancellationToken = default);
	Task<string> UpdateDatabaseAsync(CancellationToken cancellationToken = default);
}
