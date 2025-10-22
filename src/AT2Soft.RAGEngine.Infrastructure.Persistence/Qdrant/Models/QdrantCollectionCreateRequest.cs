using System;

namespace AT2Soft.RAGEngine.Infrastructure.Persistence.Qdrant.Models;

public class QdrantCollectionCreateRequest
{
    public QdrantVector Vectors { get; set; } = new();
}
