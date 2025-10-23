using AT2Soft.RAGEngine.Application.Persistence.Interfaces;
using AT2Soft.RAGEngine.Domain.Entities;
using AT2Soft.RAGEngine.Infrastructure.SQLServer.Data;
using Microsoft.EntityFrameworkCore;

namespace AT2Soft.RAGEngine.Infrastructure.SQLServer.Repository;

public class ChunkRepository(RAGSqlServerDbContext context) : RepositoryBase<Chunk, int>(context), IChunkRepository
{
    public Task AddRangeAsync(List<Chunk> chunks, CancellationToken cancellationToken = default)
    {
        return _context.Chunks
            .AddRangeAsync(chunks, cancellationToken);
    }

    public Task<Chunk?> GetByPointIdAsync(Guid chunkId, CancellationToken cancellationToken = default)
    {
        return _context.Chunks
            .Include(c => c.KnowledgeDocument)
            .ThenInclude(kd => kd!.Chunks)
            .FirstOrDefaultAsync(c => c.KDId == chunkId, cancellationToken);
    }
}
