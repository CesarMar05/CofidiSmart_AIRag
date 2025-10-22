using System;

namespace AT2Soft.RAGEngine.Infrastructure.Persistence.Qdrant.Models;

public class QdrantSettings
{
    public string UrlBase { get; set; } = string.Empty;
    public string Collection { get; set; } = string.Empty;
}
