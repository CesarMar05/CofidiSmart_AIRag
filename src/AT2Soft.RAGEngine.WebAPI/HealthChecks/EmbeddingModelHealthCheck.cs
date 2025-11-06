using AT2Soft.RAGEngine.Domain.Interfaces.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AT2Soft.RAGEngine.WebAPI.HealthChecks;

public class EmbeddingModelHealthCheck(IAIModelService aiModel) : HealthCheckBase(3500)
{
    private readonly IAIModelService _aiModel = aiModel;

    protected override async Task<object?> ExecuteAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var rslt = await _aiModel.EmbeddingTextAsync("Esta es una prueba de embedding para HealthCheck", cancellationToken);
        return rslt;
    }

    protected override HealthCheckResult ValidateResult(HealthCheckContext context, object? result, long elapsedMilliseconds)
    {
        if (result is not float[] vectors)
            return HealthCheckResult.Unhealthy("El resultado del embedding no es válido.");

        if (vectors.Length == 0)
            return HealthCheckResult.Degraded("El servicio de embeddings respondió vacío.");

        if (vectors.Length < 1024)
            return HealthCheckResult.Degraded("El embedding parece incompleto o truncado.");

        return HealthCheckResult.Healthy("Servicio respondió en satisfactoriamente.");
    }
}
