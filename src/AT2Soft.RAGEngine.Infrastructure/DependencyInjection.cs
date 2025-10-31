using AT2Soft.RAGEngine.Application.Abstractions.Queue;
using AT2Soft.RAGEngine.Application.Abstractions.TextChunker;
using AT2Soft.RAGEngine.Application.Abstractions.TextExtractor;
using AT2Soft.RAGEngine.Domain.Interfaces.Services;
using AT2Soft.RAGEngine.Infrastructure.AIModel;
using AT2Soft.RAGEngine.Infrastructure.AIModel.DelegatingHandlers;
using AT2Soft.RAGEngine.Infrastructure.AIModel.Models;
using AT2Soft.RAGEngine.Infrastructure.Embedding;
using AT2Soft.RAGEngine.Infrastructure.Queue;
using AT2Soft.RAGEngine.Infrastructure.TextExtractors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;

namespace AT2Soft.RAGEngine.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddRAGTextExtractors(this IServiceCollection services)
    {
        services.AddSingleton<IFileTextExtractor, DocxTextExtractor>();
        services.AddSingleton<IFileTextExtractor, PdfTextExtractor>();
        services.AddSingleton<IFileTextExtractor, TxtTextExtractor>();

        services.AddSingleton<IFileTextExtractorFactory, FileTextExtractorFactory>();

        return services;
    }

    public static IServiceCollection AddRAGQueue(this IServiceCollection services)
    {
        //services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        services.AddSingleton<IBackgroundTaskQueue>(_ => new BackgroundTaskQueue(capacity: 500));

        return services;
        
    }

    public static IServiceCollection AddRAGOllama(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<OllamaOptions>()
            .Bind(configuration.GetSection("Ollama"))
            .Validate(o => !string.IsNullOrWhiteSpace(o.BaseUrl), "Ollama:BaseUrl es requerido")
            .Validate(o => !string.IsNullOrWhiteSpace(o.AIModel), "Ollama:AIModel es requerido")
            .Validate(o => !string.IsNullOrWhiteSpace(o.EmbeddingModel), "Ollama:EmbeddingModel es requerido")
            .ValidateOnStart();

        services.AddTransient<HttpLoggingHandler>();
        
        services.AddRefitClient<IOllamaClient>()
            .ConfigureHttpClient((sp, c) =>
            {
                var opts = sp.GetRequiredService<IOptions<OllamaOptions>>().Value;
                c.BaseAddress = new Uri(opts.BaseUrl);
            })
            .AddHttpMessageHandler<HttpLoggingHandler>();

        services.AddScoped<IAIModelService, OllamaService>();
        return services;
    }
    
    public static IServiceCollection AddRAGTextChunker(this IServiceCollection services)
    {   
        services.AddScoped<ITextChunkerService, TextChunkerService>();
        return services;
    }    
}
