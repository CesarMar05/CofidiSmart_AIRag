using AT2Soft.Application.Result;
using AT2Soft.RAGEngine.Application.Abstractions.KnowledgeDocument;
using AT2Soft.RAGEngine.Application.Abstractions.TextChunker;
using AT2Soft.RAGEngine.Application.Abstractions.TextExtractor;
using AT2Soft.RAGEngine.Application.Interfaces.Services;
using AT2Soft.RAGEngine.Domain.Enums;
using MediatR;

namespace AT2Soft.RAGEngine.Application.Features.KnowledgeDocument.Commands;

public sealed record KDReceiveFileCommand(KDMetadataRequest Metadata, TextChunkerOptions TextChunkerOptions, string FilaName, Stream FileContent): IRequest<Result<Guid>>;

internal class KDReceiveFileCommandHandler : IRequestHandler<KDReceiveFileCommand, Result<Guid>>
{
    private readonly IFileTextExtractorFactory _fileTextExtractorFactory;
    private readonly IRagIngestJobServices _ragIngestJobServices;

    public KDReceiveFileCommandHandler(IFileTextExtractorFactory fileTextExtractorFactory, IRagIngestJobServices ragIngestJobServices)
    {
        _fileTextExtractorFactory = fileTextExtractorFactory;
        _ragIngestJobServices = ragIngestJobServices;
    }

    public async Task<Result<Guid>> Handle(KDReceiveFileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var ext = Path.GetExtension(request.FilaName);

            var extractor = _fileTextExtractorFactory.GetByExtension(ext);
            var text = await extractor.ExtractTextAsync(request.FileContent, cancellationToken);

            var sourceType = ext switch
            {
                ".pdf" => KnowledgeDocumentType.PDF,
                ".docx" => KnowledgeDocumentType.Word,
                ".xlsx" => KnowledgeDocumentType.Excel,
                ".pptx" => KnowledgeDocumentType.PowerPoint,
                ".txt" => KnowledgeDocumentType.Text,
                _ => KnowledgeDocumentType.Otro
            };

            return await _ragIngestJobServices.AddRagIngestJob(
                request.Metadata,
                request.TextChunkerOptions,
                sourceType,
                request.FilaName,
                text,
                cancellationToken
            );

        }
        catch (Exception ex)
        {
            return Result.Failure<Guid>(new("KDReceiveFileFail", $"Fallo la Recepción del Archivo {request.FilaName}. {ex.Message}"));
        }
    }
}
