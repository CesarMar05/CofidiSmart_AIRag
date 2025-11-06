using AT2Soft.Application.Result;
using AT2Soft.RAGEngine.Application.Abstractions.KnowledgeDocument;
using AT2Soft.RAGEngine.Application.Abstractions.TextChunker;
using AT2Soft.RAGEngine.Application.Interfaces.Services;
using AT2Soft.RAGEngine.Domain.Enums;
using MediatR;

namespace AT2Soft.RAGEngine.Application.Features.KnowledgeDocument.Commands;

public sealed record KDReceiveTextCommand(KDMetadataRequest Metadata, TextChunkerOptions TextChunkerOptions, string Text): IRequest<Result<Guid>> ;

internal class KDReceiveTextCommandHandler : IRequestHandler<KDReceiveTextCommand, Result<Guid>>
{
    private readonly IRagIngestJobServices _ragIngestJobServices;

    public KDReceiveTextCommandHandler(IRagIngestJobServices ragIngestJobServices)
    {
        _ragIngestJobServices = ragIngestJobServices;
    }

    public async Task<Result<Guid>> Handle(KDReceiveTextCommand request, CancellationToken cancellationToken)
    {
        return await _ragIngestJobServices.AddRagIngestJob(
            request.Metadata,
            request.TextChunkerOptions,
            KnowledgeDocumentType.Text,
            "TextPlain",
            request.Text,
            cancellationToken
        );
    }
}