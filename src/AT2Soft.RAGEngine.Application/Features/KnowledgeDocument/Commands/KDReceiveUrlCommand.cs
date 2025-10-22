using AT2Soft.Application.Result;
using AT2Soft.RAGEngine.Application.Abstractions.KnowledgeDocument;
using AT2Soft.RAGEngine.Application.Abstractions.TextChunker;
using AT2Soft.RAGEngine.Application.Helpers;
using AT2Soft.RAGEngine.Application.Interfaces.Services;
using AT2Soft.RAGEngine.Domain.Enums;
using AT2Soft.RAGEngine.Domain.Interfaces.Services;
using MediatR;

namespace AT2Soft.RAGEngine.Application.Features.KnowledgeDocument.Commands;

public sealed record KDReceiveUrlCommand(Guid ApplicationId, KDMetadataRequest Metadata, TextChunkerOptions TextChunkerOptions, string Url) : IRequest<Result<Guid>>;

internal class KDReceiveUrlCommandHandler : IRequestHandler<KDReceiveUrlCommand, Result<Guid>>
{
    private readonly IRagIngestJobServices _ragIngestJobServices;

    public KDReceiveUrlCommandHandler(IAIModelService ollamaService, IRagIngestJobServices ragIngestJobServices)
    {
        _ragIngestJobServices = ragIngestJobServices;
    }

    public async Task<Result<Guid>> Handle(KDReceiveUrlCommand request, CancellationToken cancellationToken)
    {
        // Paso 1: Extraer Texto de Url
        string originContent = await HttpHelper.GetCleanTextFromUrlAsync(request.Url, cancellationToken);
        if (string.IsNullOrEmpty(originContent))
            return Result.Failure<Guid>(new("UrlEmptyData", $"No se obtuvo información de la Url {request.Url}"));


        // Paso 2: usar el modelo LLM para extraer el texto útil de la página
        //string cleanText = await _ollamaService.ExtractTextFromHtmlAsync(request.SourceId, "codellama:7b-instruct", originContent, cancellationToken);


        // Paso 3: insertar JOB
        return await _ragIngestJobServices.AddRagIngestJob(
            request.ApplicationId,
            request.Metadata,
            request.TextChunkerOptions,
            KnowledgeDocumentType.URL,
            request.Url,
            originContent,
            cancellationToken
        );
    }
}