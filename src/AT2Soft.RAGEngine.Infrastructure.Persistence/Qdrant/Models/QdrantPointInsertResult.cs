using System;

namespace AT2Soft.RAGEngine.Infrastructure.Persistence.Qdrant.Models;

public class QdrantPointInsertResult
{
    public int OperationId { get; set; }
    public string Status { get; set; } = string.Empty;
}
