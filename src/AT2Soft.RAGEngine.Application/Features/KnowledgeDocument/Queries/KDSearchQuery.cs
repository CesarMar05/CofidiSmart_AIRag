
using AT2Soft.Application.Result;
using AT2Soft.RAGEngine.Application.Persistence.Interfaces;
using AT2Soft.RAGEngine.Domain.Interfaces.Services;
using AT2Soft.RAGEngine.Domain.Models;
using MediatR;

namespace AT2Soft.RAGEngine.Application.Features.KnowledgeDocument.Queries;
public sealed record class KDSearchRequest(string TenantId, string Query, int TopK = 3);
public sealed record class KDSearchQuery
(
    Guid ApplicationId,
    string TenantId,
    string Query,
    int TopK = 3

) : IRequest<Result<List<VectorialSearchResult>>>;

internal sealed class KDSearchQueryHandler : IRequestHandler<KDSearchQuery, Result<List<VectorialSearchResult>>>
{
    private readonly IAIModelService _ollamaService;
    private readonly IPointRepository _pointRepository;

    public KDSearchQueryHandler(IAIModelService ollamaService, IPointRepository pointRepository)
    {
        _ollamaService = ollamaService;
        _pointRepository = pointRepository;
    }
    
    public async Task<Result<List<VectorialSearchResult>>> Handle(KDSearchQuery request, CancellationToken cancellationToken)
    {
        var embedding = await _ollamaService.EmbeddingTextAsync(request.Query, cancellationToken);

        var rslt = await _pointRepository.SearchSimilarTextsAsync(request.ApplicationId, request.TenantId,embedding, request.TopK, cancellationToken);

        return rslt;
    }
}
