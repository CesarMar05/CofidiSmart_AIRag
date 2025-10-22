using AT2Soft.Application.Result;
using AT2Soft.RAGEngine.Application.Abstractions.DTOs;
using AT2Soft.RAGEngine.Application.Abstractions.TextChunker;
using AT2Soft.RAGEngine.Domain.Enums;

namespace AT2Soft.RAGEngine.Application.Interfaces;

public interface IKnowledgeDocumentServices
{
    Task<Result<KnowledgeDocumentInfo>> InsertPlainTextAsyn(Guid appId, string tenantId, string topic, List<string> tags, KnowledgeDocumentType sourceType, string sourceId, string title, string description, string text, TextChunkerOptions textChunkerOptions, CancellationToken cancellationToken);
    Task<string> GetChunkContext(Guid chunkPointId, string chunkContent, CancellationToken cancellationToken = default);
    Task<Result<List<KnowledgeDocumentInfo>>> GetList(Guid applicationId, string tenant, int take, CancellationToken cancellationToken = default);
    Task<Result<KnowledgeDocumentInfo?>> GetById(Guid applicationId, string tenant, Guid id, bool fullData, CancellationToken cancellationToken = default);
}
