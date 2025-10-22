using AT2Soft.RAGEngine.Domain.Enums;

namespace AT2Soft.RAGEngine.Domain.Entities;

public class KnowledgeDocument
{
    public Guid Id { get; set; }
    public Guid ApplicationClientId { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public KnowledgeDocumentType Type { get; set; }
    public string Source { get; set; } = string.Empty;     //Nombre de arhivo (pdf, docs, url, etc.)
    public string Description { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string Digest { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = []; 
    public string CollectionName { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; }
    public DateTime LastUpdate { get; set; }


    public ApplicationClient ApplicationClient { get; set; } = default!;
    public List<Chunk> Chunks { get; set; } = [];
}
