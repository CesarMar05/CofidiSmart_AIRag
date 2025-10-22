using System;

namespace AT2Soft.RAGEngine.Domain.Models;

public class DocumentChunkRequest
{
    public string SourceId { get; set; } = Guid.NewGuid().ToString(); // ID del documento
    public string Text { get; set; } = string.Empty;
    public int ChunkSize { get; set; } = 500;
    public int ChunkOverlap { get; set; } = 50;
}
