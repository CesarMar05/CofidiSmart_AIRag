using AT2Soft.RAGEngine.Domain.Enums;

namespace AT2Soft.RAGEngine.Application.Abstractions.DTOs;

public sealed record RagIngestJobInfo
(
    Guid Id,
    Guid ApplicationId,
    string TenantId,
    KnowledgeDocumentType SourceType,
    string Source,
    int TextLength,
    string TextChunkerOptions,
    string Metadata,
    IngestStatus Status,
    DateTime CreatedAtUtc,
    DateTime? CompletedAtUtc,
    string? Error,
    string Digest,
    int ChunksProcess,
    int ChunksTotal
);