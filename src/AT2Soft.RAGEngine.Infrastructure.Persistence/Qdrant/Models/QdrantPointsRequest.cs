using AT2Soft.RAGEngine.Domain.Entities;

namespace AT2Soft.RAGEngine.Infrastructure.Persistence.Qdrant.Models;

public class QdrantPointsRequest
{
    public List<QdrantPoint<Payload>> Points { get; set; } = [];
}
