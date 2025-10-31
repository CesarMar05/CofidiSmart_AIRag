using System.Text.Json.Serialization;

namespace AT2Soft.RAGEngine.Infrastructure.Persistence.Qdrant.Models;


public record MatchValue([property: JsonPropertyName("value")] string Value);
public record MatchAny([property: JsonPropertyName("any")] string[] Any);
public record IsEmpty([property: JsonPropertyName("key")] string Key);



public record FieldCondition(
    [property: JsonPropertyName("key")] string? Key,
    [property: JsonPropertyName("match")] object? Match,
    [property: JsonPropertyName("is_empty")] IsEmpty? IsEmpty
)
{
    public static FieldCondition Value(string key, string value)
        => new(key, new MatchValue(value), null);

    public static FieldCondition Any(string key, params string[] any)
        => new(key, new MatchAny(any), null);

    public static FieldCondition IsEmptyField(string key) =>
        new(null, null, new IsEmpty(key) );
}

public record Filter()
{
    [JsonPropertyName("must")]      public List<FieldCondition>? Must { get; set; } = null;

    [JsonPropertyName("should")]     public List<FieldCondition>? Should { get; set; } = null;

    [JsonPropertyName("must_not")]  public List<FieldCondition>? MustNot { get; set; } = null;
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
