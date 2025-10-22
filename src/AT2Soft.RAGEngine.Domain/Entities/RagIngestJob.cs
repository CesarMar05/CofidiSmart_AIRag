using System;
using System.Globalization;
using AT2Soft.RAGEngine.Domain.Enums;

namespace AT2Soft.RAGEngine.Domain.Entities;

public class RagIngestJob
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ApplicationId { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public KnowledgeDocumentType SourceType { get; set; }
    public string Source { get; set; } = string.Empty;
    public string TextContent { get; set; } = string.Empty;
    public int TextLength { get; set; }
    public string TextChunkerOptions { get; set; } = string.Empty;
    public string Metadata { get; set; } = string.Empty;
    public IngestStatus Status { get; set; } = IngestStatus.Pending;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAtUtc { get; set; }
    public string? Error { get; set; }
    public string Digest { get; set; } = string.Empty;
    public int ChunksProcess { get; set; }
    public int ChunksTotal { get; set; }
}
