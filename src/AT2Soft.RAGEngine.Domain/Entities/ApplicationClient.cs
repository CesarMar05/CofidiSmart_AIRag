using System;

namespace AT2Soft.RAGEngine.Domain.Entities;

public class ApplicationClient
{
    public Guid ApplicationClientId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string ClientSecretHash { get; set; } = string.Empty;

    // Ej: "ingest query"
    public string AllowedScopes { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public string Prompt { get; set; } = string.Empty;

    public List<KnowledgeDocument> KnowledgeDocuments { get; set; } = [];
}
