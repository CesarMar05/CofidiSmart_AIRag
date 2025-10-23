using AT2Soft.RAGEngine.Application.Persistence.Interfaces;
using AT2Soft.RAGEngine.Infrastructure.Persistence.Data;
using AT2Soft.RAGEngine.Infrastructure.Persistence.Qdrant.APIClient;
using AT2Soft.RAGEngine.Infrastructure.Persistence.Qdrant.Models;
using AT2Soft.RAGEngine.Infrastructure.Persistence.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace AT2Soft.RAGEngine.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistenceQdrant(this IServiceCollection services, IConfiguration configuration)
    {
        // Configuration
        var qdrantSettings = configuration
            .GetSection("QdrantSettings")
            .Get<QdrantSettings>()!;

        // Registrar ApplicationDatabaseContext
        services.AddSingleton<RAGVectorDbContext>(sp =>
            new RAGVectorDbContext(qdrantSettings.UrlBase, qdrantSettings.Collection));

        // Refit API Client
        services
            .AddRefitClient<IQdrantApiClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(qdrantSettings.UrlBase));
        
        // Registrar repositorios
        services.AddSingleton<ICollectionRepository, CollectionRepository>();
        services.AddSingleton<IPointRepository, PointRepository>();

        return services;
    }
}
