using System;
using System.Text.Json.Serialization;

namespace AT2Soft.RAGEngine.Domain.Entities;

public class Payload
{
    [JsonPropertyName("application")]
    public string ApplicationId { get; set; } = string.Empty;

    [JsonPropertyName("tenant")]
    public string TenantId { get; set; } = string.Empty;

    [JsonPropertyName("topic")]
    public string Topic { get; set; } = string.Empty;

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = [];

    [JsonPropertyName("kd_id")]
    public Guid KnowledgeDocumentId { get; set; }

    [JsonPropertyName("position")]
    public int Position { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    
}
