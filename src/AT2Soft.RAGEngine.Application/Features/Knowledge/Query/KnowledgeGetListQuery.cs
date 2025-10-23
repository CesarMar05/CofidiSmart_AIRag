using AT2Soft.Application.Result;
using AT2Soft.RAGEngine.Application.Persistence.Interfaces;
using MediatR;

namespace AT2Soft.RAGEngine.Application.Features.Knowledge.Query;

public sealed record KnowledgeGetListQuery() : IRequest<Result<List<Domain.Entities.Knowledge>>>;

internal class KnowledgeGetListQueryHandler : IRequestHandler<KnowledgeGetListQuery, Result<List<Domain.Entities.Knowledge>>>
{
    private readonly IKnowledgeRepository _knowledgeRepository;

    public KnowledgeGetListQueryHandler(IKnowledgeRepository knowledgeRepository)
    {
        _knowledgeRepository = knowledgeRepository;
    }

    public async Task<Result<List<Domain.Entities.Knowledge>>> Handle(KnowledgeGetListQuery request, CancellationToken cancellationToken)
    {
        return (await _knowledgeRepository.GetListAsync(cancellationToken: cancellationToken))
            .ToList();
    }
}
