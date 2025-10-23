using AT2Soft.RAGEngine.Domain.Entities;

namespace AT2Soft.RAGEngine.Application.Persistence.Interfaces;

public interface IChunkRepository : IRepository<Chunk, int>
{
    Task AddRangeAsync(List<Chunk> chunks, CancellationToken cancellationToken = default);
    Task<Chunk?> GetByPointIdAsync(Guid chunkId, CancellationToken cancellationToken = default);
}
