using AT2Soft.RAGEngine.Application.Interfaces.Repositories;
using AT2Soft.RAGEngine.Domain.Entities;
using AT2Soft.RAGEngine.Domain.Models;
using AT2Soft.RAGEngine.Infrastructure.SQLServer.Data;
using Microsoft.EntityFrameworkCore;

namespace AT2Soft.RAGEngine.Infrastructure.SQLServer.Repository;

public class KnowledgeDocumentRepository(RAGSqlServerDbContext context) : BaseRepository<KnowledgeDocument, Guid>(context), IKnowledgeDocumentRepository
{
    public Task<bool> ExistDigest(Guid applicationId, string digest, CancellationToken cancellationToken = default)
    {
        return _context.KnowledgeDocuments
            .AnyAsync(kd => kd.ApplicationClientId == applicationId && kd.Digest == digest, cancellationToken);
    }

    public Task<KnowledgeDocument?> GetFullDataByIdAsync(Guid kdid, CancellationToken cancellationToken = default)
    {
        return _context.KnowledgeDocuments
            .Include(kd => kd.Chunks)
            .Include(kd => kd.ApplicationClient)
            .FirstOrDefaultAsync(kd => kd.Id == kdid, cancellationToken);
    }
}
