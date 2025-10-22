using AT2Soft.RAGEngine.Domain.Entities;
using AT2Soft.RAGEngine.Domain.Interfaces;

namespace AT2Soft.RAGEngine.Application.Interfaces.Repositories;

public interface IChunkRepository : IRepository<Chunk, int>
{
    Task AddRangeAsync(List<Chunk> chunks, CancellationToken cancellationToken = default);
    Task<Chunk?> GetByPointIdAsync(Guid chunkId, CancellationToken cancellationToken = default);
}
