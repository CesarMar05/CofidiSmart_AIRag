using AT2Soft.RAGEngine.Domain.Entities;

namespace AT2Soft.RAGEngine.Application.Persistence.Interfaces;

public interface IApplicationClientRepository : IRepository<ApplicationClient, Guid>
{
    Task<ApplicationClient?> GetFullDataByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistAdminAsync(CancellationToken cancellationToken = default);
    Task<ApplicationClient?> FindByName(string name, CancellationToken cancellationToken = default);
    Task<string> GetPrompt(Guid appCltId, string tenant);
}
