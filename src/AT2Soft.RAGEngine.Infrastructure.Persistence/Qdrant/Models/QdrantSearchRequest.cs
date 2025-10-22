using System;
using System.Text.Json.Serialization;

namespace AT2Soft.RAGEngine.Infrastructure.Persistence.Qdrant.Models;


public record MatchValue(string Value);
public record FieldCondition(string Key, MatchValue Match);
public record Filter()
{
    public List<FieldCondition>? Must { get; set; } = null;

    public List<FieldCondition>? Should { get; set; } = null;

    [JsonPropertyName("must_not")]
    public List<FieldCondition>? MustNot { get; set; } = null;
}

public class QdrantSearchRequest
{
    public float[] Vector { get; set; } = [];
    public int Limit { get; set; } = 5;

    [JsonPropertyName("with_payload")]
    public bool WithPayload { get; set; } = true;

    [JsonPropertyName("with_vector")]
    public bool WithVector { get; set; } = false;
    public Filter? Filter { get; set; } = null;
}
