using System;

namespace AT2Soft.RAGEngine.Infrastructure.AIModel;

public class OllamaEmbeddingRequest
{
    public string Model { get; set; } = "nomic-embed-text";
    public string Prompt { get; set; } = string.Empty;
}
