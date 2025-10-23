using AT2Soft.RAGEngine.Application.Abstractions.DTOs;
using AT2Soft.RAGEngine.Domain.Entities;

namespace AT2Soft.RAGEngine.Application.Persistence.Interfaces;

public interface IRagIngestJobRepository : IRepository<RagIngestJob, Guid>
{
    Task<bool> ExistDigest(Guid applicationId, string digest, CancellationToken cancellationToken = default);
    Task<RagIngestJobInfo?> FindRagIngestJobInfoByIdAsync(Guid appId, string tenantId, Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<RagIngestJobInfo>> GetRagIngestJobInfoListAsync(Guid appId, string tenantId, CancellationToken cancellationToken = default);
}
