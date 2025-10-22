using AT2Soft.RAGEngine.Domain.Models;
using Refit;

namespace AT2Soft.RAGEngine.Infrastructure.AIModel;

public interface IOllamaClient
{
    [Post("/api/generate")]
    Task<AIModelResponse> AskModel(AIModelRequest request, CancellationToken cancellationToken = default);

    [Post("/api/embeddings")]
    Task<OllamaEmbeddingResponse> CreateEmbedding(OllamaEmbeddingRequest request, CancellationToken cancellationToken = default);
}
