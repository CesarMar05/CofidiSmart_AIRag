
using AT2Soft.Application.Result;
using AT2Soft.RAGEngine.Application.Abstractions.Authentication;
using AT2Soft.RAGEngine.Application.Persistence.Interfaces;
using AT2Soft.RAGEngine.Domain.Interfaces.Services;
using AT2Soft.RAGEngine.Domain.Models;
using MediatR;

namespace AT2Soft.RAGEngine.Application.Features.KnowledgeDocument.Queries;
//public sealed record class KDSearchRequest(string Query, int TopK = 3);
public sealed record class KDSearchQuery
(
    string Query,
    int TopK = 3

) : IRequest<Result<List<VectorialSearchResult>>>;

internal sealed class KDSearchQueryHandler : IRequestHandler<KDSearchQuery, Result<List<VectorialSearchResult>>>
{
    private readonly IAIModelService _ollamaService;
    private readonly IPointRepository _pointRepository;
    private readonly IClientContext _clientContext;

    public KDSearchQueryHandler(IAIModelService ollamaService, IPointRepository pointRepository, IClientContext clientContext)
    {
        _ollamaService = ollamaService;
        _pointRepository = pointRepository;
        _clientContext = clientContext;
    }

    public async Task<Result<List<VectorialSearchResult>>> Handle(KDSearchQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_clientContext.Tenant) || string.IsNullOrWhiteSpace(_clientContext.ClientId))
            return Result.Failure<List<VectorialSearchResult>>(new("Unauthorizer", "Invalid Torken"));

        var embedding = await _ollamaService.EmbeddingTextAsync(request.Query, cancellationToken);

        var rslt = await _pointRepository.SearchSimilarTextsAsync(new Guid(_clientContext.ClientId), _clientContext.Tenant, _clientContext.Divisions, embedding, request.TopK, cancellationToken);

        return rslt;
    }
}
