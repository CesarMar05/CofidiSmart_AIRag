using AT2Soft.Application.Result;
using AT2Soft.RAGEngine.Application.Abstractions.DTOs;
using AT2Soft.RAGEngine.Application.Interfaces;
using MediatR;

namespace AT2Soft.RAGEngine.Application.Features.KnowledgeDocument.Queries;

public sealed record class KDGetByIdQuery(Guid ApplicationId, string TenantId, Guid KdId, bool FullData = false) : IRequest<Result<KnowledgeDocumentInfo>>;

internal class KDGetByIdQueryHandler : IRequestHandler<KDGetByIdQuery, Result<KnowledgeDocumentInfo>>
{
    private readonly IKnowledgeDocumentServices _kdService;

    public KDGetByIdQueryHandler(IKnowledgeDocumentServices kdService)
    {
        _kdService = kdService;
    }

    public async Task<Result<KnowledgeDocumentInfo>> Handle(KDGetByIdQuery request, CancellationToken cancellationToken)
    {
        var kd = await _kdService.GetById(request.ApplicationId, request.TenantId, request.KdId, request.FullData, cancellationToken);
        if (kd.IsFailure || kd.Value == null)
            return Result.Failure<KnowledgeDocumentInfo>(new("KnowledgeDocumentNotFound", $"No se localizo el KnowledgeDocument con Id {request.KdId}"));

        return kd.Value;
    }
}
