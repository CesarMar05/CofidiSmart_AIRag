
namespace AT2Soft.RAGEngine.Domain.Entities;

public class Chunk
{
    public Guid Id { get; set; }
    public Guid KDId { get; set; }
    public int Position { get; set; }
    public string Content { get; set; } = string.Empty; 
    public int EstimatedTokens { get; set; }

    public KnowledgeDocument? KnowledgeDocument { get; set; }
}
