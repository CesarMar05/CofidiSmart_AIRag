using AT2Soft.RAGEngine.Application.Interfaces;
using AT2Soft.RAGEngine.Application.Persistence.Interfaces;
using AT2Soft.RAGEngine.Application.Persistence.Migrations;
using AT2Soft.RAGEngine.Infrastructure.SQLServer.Data;
using AT2Soft.RAGEngine.Infrastructure.SQLServer.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AT2Soft.RAGEngine.Infrastructure.SQLServer;

public static class DependencyInjection
{
    public static IServiceCollection AddRAGSqlServer(this IServiceCollection services, string connectionString)
    {
        // Registrar DbContext SQL Server
        services.AddDbContext<RAGSqlServerDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Registrar UnitOfWork
        services.AddScoped<IUnitOfWorkRelational, UnitOfWorkSqlServer>();

        
        // Registrar repositorios
        services.AddScoped<IKnowledgeDocumentRepository, KnowledgeDocumentRepository>();
        services.AddScoped<IKnowledgeRepository, KnowledgeRepository>();
        services.AddScoped<IChunkRepository, ChunkRepository>();
        services.AddScoped<IApplicationClientRepository, ApplicationClientRepository>();
        services.AddScoped<IRagIngestJobRepository, RagIngestJobRepository>();
        services.AddScoped<IMigrationRepository, MigrationRepository>();

        return services;
    }
}
