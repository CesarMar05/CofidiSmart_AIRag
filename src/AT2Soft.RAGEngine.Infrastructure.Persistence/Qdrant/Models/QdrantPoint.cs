using System;

namespace AT2Soft.RAGEngine.Infrastructure.Persistence.Qdrant.Models;

public class QdrantPoint<T>
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public float[] Vector { get; set; } = [];
    public T Payload { get; set; } = default!;
}
