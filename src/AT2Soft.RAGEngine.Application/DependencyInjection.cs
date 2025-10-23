using System.Reflection;
using AT2Soft.RAGEngine.Application.Features.AppClient.Services;
using AT2Soft.RAGEngine.Application.Features.Ingest.Services;
using AT2Soft.RAGEngine.Application.Features.KnowledgeDocument.Services;
using AT2Soft.RAGEngine.Application.Features.Prompt.Interfaces;
using AT2Soft.RAGEngine.Application.Features.Prompt.Services;
using AT2Soft.RAGEngine.Application.Features.RagIngestJob.Services;
using AT2Soft.RAGEngine.Application.Interfaces;
using AT2Soft.RAGEngine.Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AT2Soft.RAGEngine.Application;

public class ApplicationAssemblyReference
{
    internal static readonly Assembly Assembly = typeof(ApplicationAssemblyReference).Assembly;
}

public static class DependencyInjection
{
    public static IServiceCollection AddRAGApplication(this IServiceCollection services)
    {
        // Registrar MediatR
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<ApplicationAssemblyReference>());

        // Registrar Services
        services.AddScoped<IKnowledgeDocumentServices, KnowledgeDocumentServices>();
        services.AddScoped<IApplicationClientService, ApplicationClientService>();
        services.AddScoped<IRagIngestJobServices, RagIngestJobServices>();
        services.AddScoped<IIngestTextService, IngestTextService>();
        services.AddScoped<IPromptServices, PromptServices>();

        return services;
    }
}
