using System.ComponentModel.DataAnnotations;
using AT2Soft.Application.Result;
using AT2Soft.RAGEngine.Application.Interfaces;
using AT2Soft.RAGEngine.Application.Interfaces.Repositories;
using MediatR;

namespace AT2Soft.RAGEngine.Application.Features.Knowledge.Commands;

public sealed record KnowledgeAddCommand(
    [Required,MaxLength(12), MinLength(3)]string Id,
    [Required,MaxLength(150), MinLength(3)]string Description) : IRequest<Result>;

internal class KnowledgeAddCommandHandler : IRequestHandler<KnowledgeAddCommand, Result>
{
    private IKnowledgeRepository _knowledgeRepository;
    private IUnitOfWorkRelational _unitOfWork;

    public KnowledgeAddCommandHandler(IKnowledgeRepository knowledgeRepository, IUnitOfWorkRelational unitOfWork)
    {
        _knowledgeRepository = knowledgeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(KnowledgeAddCommand request, CancellationToken cancellationToken)
    {
        var kn = new Domain.Entities.Knowledge
        {
            KnowledgeId = 0,
            Name = request.Id,
            Description = request.Description
        };

        await _knowledgeRepository.AddAsync(kn, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
