using AT2Soft.RAGEngine.Domain.Interfaces.Services;
using AT2Soft.RAGEngine.Domain.Models;
using AT2Soft.RAGEngine.Infrastructure.AIModel.Models;
using Microsoft.Extensions.Options;

namespace AT2Soft.RAGEngine.Infrastructure.AIModel;

public class OllamaService : IAIModelService
{
    private readonly IOllamaClient _ollamaClient;
    private readonly IOptionsMonitor<OllamaOptions> _options;

    public OllamaService(IOllamaClient ollamaClient, IOptionsMonitor<OllamaOptions> options)
    {
        _ollamaClient = ollamaClient;
        _options = options;
    }

    public async Task<string> AskModelAsync(string prompt, CancellationToken cancellationToken = default)
    {
        var request = new AIModelRequest
        {
            Model = _options.CurrentValue.AIModel,
            Prompt = prompt,
            Stream = false
        };

        var response = await _ollamaClient.AskModel(request, cancellationToken);
        return response?.Response ?? string.Empty;
    }

    public async Task<List<float[]>> EmbeddingTextListAsync(List<string> chunks, CancellationToken cancellationToken = default)
    {
        var model = _options.CurrentValue.EmbeddingModel;
        var vectors = new List<float[]>(chunks.Count);

        foreach (var chunk in chunks)
        {
            var req = new OllamaEmbeddingRequest { Model = model, Prompt = chunk };
            var res = await _ollamaClient.CreateEmbedding(req, cancellationToken)
                      ?? throw new Exception("Sin respuesta de la API de embeddings.");
            vectors.Add(res.Embedding);
        }
        return vectors;
    }

    public async Task<float[]> EmbeddingTextAsync(string text, CancellationToken cancellationToken = default)
    {
        var req = new OllamaEmbeddingRequest
        {
            Model = _options.CurrentValue.EmbeddingModel,
            Prompt = text
        };

        var res = await _ollamaClient.CreateEmbedding(req, cancellationToken)
                  ?? throw new Exception("Sin respuesta de la API de embeddings.");
        return res.Embedding;
    }
    
    public async Task<string> ExtractTextFromHtmlAsync(string title, string html, CancellationToken cancellationToken = default)
    {
        var req = new AIModelRequest
        {
            Model  = _options.CurrentValue.AIModel,
            Prompt = $"Analiza el contenido del siguiente texto HTML y crea un resumen para el tema \"{title}\".\n\n[HTML]\n{html}",
            Stream = false
        };

        var response = await _ollamaClient.AskModel(req, cancellationToken);
        return response?.Response ?? string.Empty;
    }
}
