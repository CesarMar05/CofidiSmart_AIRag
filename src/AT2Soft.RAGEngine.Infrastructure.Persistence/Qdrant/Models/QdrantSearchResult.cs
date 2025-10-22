using AT2Soft.RAGEngine.Domain.Entities;

namespace AT2Soft.RAGEngine.Infrastructure.Persistence.Qdrant.Models;

public class QdrantSearchResult<T>
{
    public Guid Id { get; set; }
    public int Version { get; set; }
    public double Score { get; set; }
    public T? Payload { get; set; } 
}
