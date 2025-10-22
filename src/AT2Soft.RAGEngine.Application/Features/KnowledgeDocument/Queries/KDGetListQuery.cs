using AT2Soft.Application.Result;
using AT2Soft.RAGEngine.Application.Abstractions.DTOs;
using AT2Soft.RAGEngine.Application.Interfaces;
using MediatR;

namespace AT2Soft.RAGEngine.Application.Features.KnowledgeDocument.Queries;

public sealed record KDGetListQuery(Guid ApplicationId, string TenantId, int Take = 0) : IRequest<Result<List<KnowledgeDocumentInfo>>>;

internal sealed class KDGetListQueryHandler : IRequestHandler<KDGetListQuery, Result<List<KnowledgeDocumentInfo>>>
{
    private readonly IKnowledgeDocumentServices _kdService;

    public KDGetListQueryHandler(IKnowledgeDocumentServices kdService)
    {
        _kdService = kdService;
    }

    public async Task<Result<List<KnowledgeDocumentInfo>>> Handle(KDGetListQuery request, CancellationToken cancellationToken)
    {
        return await _kdService.GetList(request.ApplicationId, request.TenantId, request.Take, cancellationToken);
    }
}