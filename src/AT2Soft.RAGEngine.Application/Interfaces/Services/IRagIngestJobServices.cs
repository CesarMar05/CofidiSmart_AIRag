using AT2Soft.Application.Result;
using AT2Soft.RAGEngine.Application.Abstractions.DTOs;
using AT2Soft.RAGEngine.Application.Abstractions.KnowledgeDocument;
using AT2Soft.RAGEngine.Application.Abstractions.TextChunker;
using AT2Soft.RAGEngine.Domain.Enums;

namespace AT2Soft.RAGEngine.Application.Interfaces.Services;

public interface IRagIngestJobServices
{
    Task<Result<Guid>> AddRagIngestJob(Guid applicationId, KDMetadataRequest metadata, TextChunkerOptions textChunkerOptions, KnowledgeDocumentType sourceType, string source, string Text, CancellationToken cancellationToken = default);
    Task<Result<RagIngestJobInfo>> GetRagIngestJobInfo(Guid appId, string tenantId, Guid jobId, CancellationToken cancellationToken);
    Task<Result<List<RagIngestJobInfo>>> GetRagIngestJobInfoList(Guid appId, string tenantId, CancellationToken cancellationToken);
}
