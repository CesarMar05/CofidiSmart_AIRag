using AT2Soft.RAGEngine.Application.Persistence.Interfaces;
using AT2Soft.RAGEngine.Domain.Entities;
using AT2Soft.RAGEngine.Infrastructure.SQLServer.Data;
using Microsoft.EntityFrameworkCore;

namespace AT2Soft.RAGEngine.Infrastructure.SQLServer.Repository;

public class ApplicationClientRepository(RAGSqlServerDbContext context) : RepositoryBase<ApplicationClient, Guid>(context), IApplicationClientRepository
{
    public async Task<ApplicationClient?> GetFullDataByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ApplicationClients
            .Include(ac => ac.KnowledgeDocuments)
            .FirstOrDefaultAsync(ac => ac.ApplicationClientId == id, cancellationToken);
    }

    public async Task<bool> ExistAdminAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ApplicationClients
            .AnyAsync(ac => ac.AllowedScopes.Contains("admin"), cancellationToken);
    }

    public async Task<ApplicationClient?> FindByName(string name, CancellationToken cancellationToken = default)
    {
        return await _context.ApplicationClients
            .FirstOrDefaultAsync(ac => ac.Name == name, cancellationToken);
    }

    public async Task<string> GetPrompt(Guid appCltId, string tenant)
    {
        var found = await _context.ApplicationClientPrompts
            .FirstOrDefaultAsync(acp => acp.ApplicationClientId == appCltId && acp.Tenant == tenant);

        return found != null
            ? found.Prompt
            : string.Empty;
    }
}
