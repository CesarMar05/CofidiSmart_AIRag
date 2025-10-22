using AT2Soft.RAGEngine.Domain.Entities;
using AT2Soft.RAGEngine.Domain.Interfaces;
using AT2Soft.RAGEngine.Domain.Models;

namespace AT2Soft.RAGEngine.Application.Interfaces.Repositories;

public interface IKnowledgeDocumentRepository : IRepository<KnowledgeDocument, Guid>
{
    Task<bool> ExistDigest(Guid applicationId, string digest, CancellationToken cancellationToken = default);
    Task<KnowledgeDocument?> GetFullDataByIdAsync(Guid kdid, CancellationToken cancellationToken = default);
}
