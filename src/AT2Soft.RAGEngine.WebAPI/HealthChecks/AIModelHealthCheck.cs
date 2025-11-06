using AT2Soft.RAGEngine.Domain.Interfaces.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AT2Soft.RAGEngine.WebAPI.HealthChecks;

public class AIModelHealthCheck(IAIModelService aiModel) : HealthCheckBase(3500)
{
    private readonly IAIModelService _aiModel = aiModel;

    protected override async Task<object?> ExecuteAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var rslt = await _aiModel.AskModelAsync("Hola", cancellationToken);
        return rslt;
    }
}
