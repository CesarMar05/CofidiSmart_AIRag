using System;

namespace AT2Soft.RAGEngine.Infrastructure.Persistence.Qdrant.Models;

public class QdrantCollectionGetListResponse
{
    public List<QdrantCollection> Collections { get; set; } = [];
}

public class QdrantCollection
{
    public string Name { get; set; } = string.Empty;
}