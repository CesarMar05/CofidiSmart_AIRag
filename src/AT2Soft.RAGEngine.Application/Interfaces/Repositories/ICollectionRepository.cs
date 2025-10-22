using AT2Soft.RAGEngine.Domain.Entities;
using AT2Soft.RAGEngine.Domain.Enums;

namespace AT2Soft.RAGEngine.Application.Interfaces.Repositories;

public interface ICollectionRepository
{
    Task<List<string>> GetListAsync(CancellationToken cancellationToken = default);
    Task<bool> CreateAsync(string collectionName, int vectorSize, DistanceType vectorDistance, CancellationToken cancellationToken = default);
}
