using System;

namespace AT2Soft.RAGEngine.WebAPI.HealthChecks;

public static class HealthChecksDependencyInjection
{
    public static IServiceCollection AddRAGHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var sqlServerCnt = configuration.GetConnectionString("RelationalDb") ?? string.Empty;

        services.AddHealthChecks()
            .AddSqlServer(sqlServerCnt)
            .AddCheck<VectorDatabaseHealthCheck>("VectorDatabase")
            .AddCheck<AIModelHealthCheck>("AI_Model")
            .AddCheck<EmbeddingModelHealthCheck>("Embedding_Model");

        return services;
    }
}
