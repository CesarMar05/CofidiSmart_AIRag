using System;

namespace AT2Soft.RAGEngine.Infrastructure.AIModel.Models;

public class OllamaOptions
{
    public string BaseUrl { get; init; } = "http://localhost:11434";            // Modelo para chat/ask
    public string AIModel { get; init; } = "llama3";            // Modelo para chat/ask
    public string EmbeddingModel { get; init; } = "nomic-embed-text"; // Modelo para embeddings
}
