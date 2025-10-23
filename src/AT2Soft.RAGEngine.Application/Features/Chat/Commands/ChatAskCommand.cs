using AT2Soft.Application.Result;
using AT2Soft.RAGEngine.Application.Abstractions.WebApiDTOs;
using AT2Soft.RAGEngine.Application.Features.Prompt.Functions;
using AT2Soft.RAGEngine.Application.Features.Prompt.Interfaces;
using AT2Soft.RAGEngine.Application.Interfaces;
using AT2Soft.RAGEngine.Application.Persistence.Interfaces;
using AT2Soft.RAGEngine.Domain.Interfaces.Services;
using MediatR;

namespace AT2Soft.RAGEngine.Application.Features.Chat.Commands;


public sealed record ChatAskCommand(Guid ApplicationId, string Tenant, string Question, bool SearchContext = true) : IRequest<Result<ChatAskResponse>>;

internal sealed class ChatAskCommandHandler : IRequestHandler<ChatAskCommand, Result<ChatAskResponse>>
{
    private readonly IAIModelService _ollamaService;
    private readonly IPointRepository _pointRepository;
    private readonly IKnowledgeDocumentServices _kdServices;
    private readonly IPromptServices _promptServices;

    public ChatAskCommandHandler(IAIModelService ollamaService, IPointRepository pointRepository, IKnowledgeDocumentServices kdServices, IPromptServices promptServices)
    {
        _ollamaService = ollamaService;
        _pointRepository = pointRepository;
        _kdServices = kdServices;
        _promptServices = promptServices;
    }

    public async Task<Result<ChatAskResponse>> Handle(ChatAskCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
            return Result.Failure<ChatAskResponse>(new("QuestionEmpty", "El prompt no puede estar vacío."));

        var embedding = await _ollamaService.EmbeddingTextAsync(request.Question, cancellationToken);

        var fullprompt = request.Question;
        var usedRag = false;
        if (request.SearchContext)
        {
            List<string> context = [];
            var searchRslt = await _pointRepository.SearchSimilarTextsAsync(request.ApplicationId, request.Tenant, embedding, 5, cancellationToken);
            foreach (var sr in searchRslt)
            {
                if (sr.Payload != null)
                    context.Add(await _kdServices.GetChunkContext(sr.Id, sr.Payload.Text, cancellationToken));
            }

            if (context.Count > 0)
            {
                fullprompt = await _promptServices.GetPrompt(request.ApplicationId, request.Question, context, cancellationToken);
                usedRag = true;
            }
        }

        var answer = await _ollamaService.AskModelAsync(fullprompt, cancellationToken);

        return string.IsNullOrWhiteSpace(answer)
            ? Result.Failure<ChatAskResponse>(new("ChatNoResponse", "No se recibió respuesta del Model"))
            : Result.Success(new ChatAskResponse(answer, usedRag));
    }
}