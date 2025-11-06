using AT2Soft.Application.Result;
using AT2Soft.RAGEngine.Application.Interfaces.Services;
using MediatR;

namespace AT2Soft.RAGEngine.Application.Features.AppClient.Commands;

public sealed record  ApplicationCltSetRAGConfigCommand(Guid ApplicationId, string Tenant, string Prompt, int Tokens, int MaxTokens, int MinTokens, int OverlapTokens) : IRequest<Result>;

internal class ApplicationCltSetPromptCommandHandler : IRequestHandler<ApplicationCltSetRAGConfigCommand, Result>
{
    private readonly IApplicationClientService _appCltService;

    public ApplicationCltSetPromptCommandHandler(IApplicationClientService appCltService)
    {
        _appCltService = appCltService;
    }

    public async Task<Result> Handle(ApplicationCltSetRAGConfigCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Prompt))
            return Result.Failure(new("PromptEmptyOrNull", "El Prompt no puede ser nulo o vacío"));
            
        return await _appCltService.SetRAGConfig(request.ApplicationId, request.Tenant, request.Prompt, request.Tokens, request.MaxTokens, request.MinTokens, request.OverlapTokens, cancellationToken);
    }
}
