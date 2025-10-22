using System;

namespace AT2Soft.RAGEngine.Infrastructure.Persistence.Qdrant.Models;

public class QdrantResponse<T>
{
    public T? Result { get; set; }
    public string? Status { get; set; } = string.Empty;
    public double Time { get; set; }
    
}

public class QdrantResponseError
{
    public QdrantResponseStatus? Status { get; set; }
    public double Time { get; set; }
}

