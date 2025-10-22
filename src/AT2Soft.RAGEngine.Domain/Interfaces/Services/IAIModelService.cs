using System;

namespace AT2Soft.RAGEngine.Domain.Interfaces.Services;

public interface IAIModelService
{
    Task<string> AskModelAsync(string prompt, CancellationToken cancellationToken = default);
    Task<List<float[]>> EmbeddingTextListAsync(List<string> chunks, CancellationToken cancellationToken = default);
    Task<float[]> EmbeddingTextAsync(string text, CancellationToken cancellationToken = default);
    Task<string> ExtractTextFromHtmlAsync(string title, string html, CancellationToken cancellationToken = default);
}
