
using AT2Soft.RAGEngine.Infrastructure.Persistence.Qdrant.APIClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AT2Soft.RAGEngine.WebAPI.HealthChecks;

public class VectorDatabaseHealthCheck(IQdrantApiClient qdrantApiClient) : HealthCheckBase()
{
    private readonly IQdrantApiClient _qdrantApiClient = qdrantApiClient;

    protected override async Task<object?> ExecuteAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var rslt = await _qdrantApiClient.CollectionGetList();
        return rslt;
    }
}
