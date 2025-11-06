using System;
using System.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AT2Soft.RAGEngine.WebAPI.HealthChecks;

public abstract class HealthCheckBase(int maxAllowedMilliseconds = 2000) : IHealthCheck
{
    private readonly int _maxAllowedMilliseconds = maxAllowedMilliseconds;

    /// <summary>
    /// Método que las clases hijas deben implementar con la lógica del HealthCheck específico.
    /// </summary>
    protected abstract Task<object?> ExecuteAsync(HealthCheckContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Método opcional para validaciones adicionales sobre el resultado de la ejecución.
    /// </summary>
    protected virtual HealthCheckResult ValidateResult(HealthCheckContext context, object? result, long elapsedMilliseconds)
    {
        return HealthCheckResult.Healthy("Servicio respondió en satisfactoriamente.");
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var result = await ExecuteAsync(context, cancellationToken);
            stopwatch.Stop();
            var elapsed = stopwatch.ElapsedMilliseconds;
            if (elapsed > _maxAllowedMilliseconds)
                return HealthCheckResult.Degraded($"Servicio respondió en {elapsed} ms (límite: {_maxAllowedMilliseconds} ms)");

            return ValidateResult(context, result, elapsed);
        }
        catch (TimeoutException ex)
        {
            return HealthCheckResult.Degraded("El servicio respondió, pero con latencia alta.", ex);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("El servicio de embeddings falló completamente.", ex);
        }
    }
}
