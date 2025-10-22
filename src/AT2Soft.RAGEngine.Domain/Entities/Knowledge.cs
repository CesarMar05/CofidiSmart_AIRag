using System;
using System.Globalization;

namespace AT2Soft.RAGEngine.Domain.Entities;

public class Knowledge
{
    public int KnowledgeId { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
