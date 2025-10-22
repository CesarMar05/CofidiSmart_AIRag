using System;
using AT2Soft.RAGEngine.Domain.Entities;
using AT2Soft.RAGEngine.Domain.Interfaces;

namespace AT2Soft.RAGEngine.Application.Interfaces.Repositories;

public interface IApplicationClientRepository : IRepository<ApplicationClient, Guid>
{
    Task<ApplicationClient?> GetFullDataByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistAdminAsync(CancellationToken cancellationToken = default);
    Task<ApplicationClient?> FindByName(string name, CancellationToken cancellationToken = default);
}
