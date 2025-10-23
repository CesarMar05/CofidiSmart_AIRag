using AT2Soft.RAGEngine.Domain.Entities;

namespace AT2Soft.RAGEngine.Application.Persistence.Interfaces;

public interface IKnowledgeDocumentRepository : IRepository<KnowledgeDocument, Guid>
{
    Task<bool> ExistDigest(Guid applicationId, string digest, CancellationToken cancellationToken = default);
    Task<KnowledgeDocument?> GetFullDataByIdAsync(Guid kdid, CancellationToken cancellationToken = default);
}
