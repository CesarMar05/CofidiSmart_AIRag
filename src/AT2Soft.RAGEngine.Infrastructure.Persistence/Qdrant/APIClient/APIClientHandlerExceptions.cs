using System;
using System.Text.Json;
using AT2Soft.RAGEngine.Infrastructure.Persistence.Qdrant.Models;
using Refit;

namespace AT2Soft.RAGEngine.Infrastructure.Persistence.Qdrant.APIClient;

public static class APIClientHandlerExceptions
{
    public static QdrantResponse<T> ApiExceptionHandler<T>(ApiException ex)
    {
        if (string.IsNullOrWhiteSpace(ex.Content))
        {
            return new QdrantResponse<T>
            {
                Status = "No content in error response",
                Result = default,
                Time = 0
            };
        }

        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var err = JsonSerializer.Deserialize<QdrantResponseError>(ex.Content, options);

            return new QdrantResponse<T>
            {
                Status = err?.Status?.Error ?? "Unknown error",
                Result = default,
                Time = err?.Time ?? 0
            };
        }
        catch (JsonException jsonEx)
        {
            return new QdrantResponse<T>
            {
                Status = $"Deserialization error: {jsonEx.Message}",
                Result = default,
                Time = 0
            };
        }
    }

    public static QdrantResponse<T> ExceptionHandler<T>(Exception ex)
    {
        return new QdrantResponse<T>
            {
                Status = $"Exception: {ex.Message}",
                Result = default,
                Time = 0
            };
    }
}
