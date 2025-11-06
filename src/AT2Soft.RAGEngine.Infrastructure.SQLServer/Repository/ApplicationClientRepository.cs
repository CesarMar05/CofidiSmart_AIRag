using AT2Soft.RAGEngine.Application.Persistence.Interfaces;
using AT2Soft.RAGEngine.Domain.Entities;
using AT2Soft.RAGEngine.Infrastructure.SQLServer.Data;
using Microsoft.EntityFrameworkCore;

namespace AT2Soft.RAGEngine.Infrastructure.SQLServer.Repository;

public class ApplicationClientRepository(RAGSqlServerDbContext context) : RepositoryBase<ApplicationClient, Guid>(context), IApplicationClientRepository
{
    public Task<ApplicationClient?> GetFullDataByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.ApplicationClients
            .Include(ac => ac.KnowledgeDocuments)
            .FirstOrDefaultAsync(ac => ac.ApplicationClientId == id, cancellationToken);

    public Task<bool> ExistAdminAsync(CancellationToken cancellationToken = default) =>
        _context.ApplicationClients
            .AnyAsync(ac => ac.AllowedScopes.Contains("admin"), cancellationToken);

    public Task<ApplicationClient?> FindByName(string name, CancellationToken cancellationToken = default) =>
        _context.ApplicationClients
            .FirstOrDefaultAsync(ac => ac.Name == name, cancellationToken);

    public Task<ApplicationClientRAGConfig?> GetApplicationClientRAGConfig(Guid appCltId, string tenant, CancellationToken cancellationToken = default) =>
        _context.ApplicationClientRAGConfigs
            .FirstOrDefaultAsync(acp => acp.ApplicationClientId == appCltId && acp.Tenant == tenant, cancellationToken);

    public async Task<ApplicationClientRAGConfig> AddApplicationClientRAGConfig(ApplicationClientRAGConfig acp, CancellationToken cancellationToken = default)
    {
        return (await _context.ApplicationClientRAGConfigs
             .AddAsync(acp, cancellationToken))
             .Entity;
    }
    
}
