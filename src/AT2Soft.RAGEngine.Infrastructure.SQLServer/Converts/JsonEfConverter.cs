using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AT2Soft.RAGEngine.Infrastructure.SQLServer.Converts;

public static class JsonEf
{
    // Opciones por defecto (ajústalas a tu gusto)
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        WriteIndented = false
        // PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        // DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    /// Converter genérico para List<T> -> string (JSON)
    public static ValueConverter<List<T>, string> ListConverter<T>(JsonSerializerOptions? options = null)
        => new(
            v => JsonSerializer.Serialize(v, options ?? DefaultOptions),
            v => string.IsNullOrWhiteSpace(v) ? new List<T>() : JsonSerializer.Deserialize<List<T>>(v, options ?? DefaultOptions)!);

    /// Comparar para que EF Core rastree cambios en List<T>
    public static ValueComparer<List<T>> ListComparer<T>()
        => new(
            (a, b) =>ReferenceEquals(a, b)
                      || (a != null && b != null
                          && a.Count == b.Count
                          && a.SequenceEqual(b)),
            v =>v == null
                 ? 0
                 : v.Aggregate(0, (acc, x) => HashCode.Combine(acc, x == null ? 0 : x.GetHashCode())),
            v => v == null ? new List<T>() : v.ToList()  // snapshot (copia)
        );

    /// Extension para aplicar conversión y comparer en una sola línea
    public static PropertyBuilder<List<T>> HasJsonListConversion<T>(
        this PropertyBuilder<List<T>> builder,
        JsonSerializerOptions? options = null)
    {
        builder.HasConversion(ListConverter<T>(options));
        builder.Metadata.SetValueComparer(ListComparer<T>());
        return builder;
    }
}