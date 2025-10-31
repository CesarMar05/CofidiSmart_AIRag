using System;
using AT2Soft.Application.Result;
using AT2Soft.RAGEngine.Application.Interfaces.Services;
using MediatR;

namespace AT2Soft.RAGEngine.Application.Features.AppClient.Commands;

public sealed record  ApplicationCltSetPromptCommand(Guid ApplicationId, string Tenant, string Prompt) : IRequest<Result>;

internal class ApplicationCltSetPromptCommandHandler : IRequestHandler<ApplicationCltSetPromptCommand, Result>
{
    private readonly IApplicationClientService _appCltService;

    public ApplicationCltSetPromptCommandHandler(IApplicationClientService appCltService)
    {
        _appCltService = appCltService;
    }

    public async Task<Result> Handle(ApplicationCltSetPromptCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Prompt))
            return Result.Failure(new("PromptEmptyOrNull", "El Prompt no puede ser nulo o vacío"));
            
        return await _appCltService.SetPrompt(request.ApplicationId, request.Tenant, request.Prompt, cancellationToken);
    }
}
