using AT2Soft.Application.Result;
using AT2Soft.RAGEngine.Application.Abstractions.Authentication;
using AT2Soft.RAGEngine.Application.Abstractions.Features.AppClient.DTOs;
using AT2Soft.RAGEngine.Application.Interfaces.Services;
using MediatR;

namespace AT2Soft.RAGEngine.Application.Features.AppClient.Queries;

public sealed record ApplicationCltGetRAGConfigQuery() : IRequest<Result<RAGConfigDto?>>;

internal class ApplicationCltGetRAGConfigQueryHandler : IRequestHandler<ApplicationCltGetRAGConfigQuery, Result<RAGConfigDto?>>
{
    private readonly IClientContext _clientContext;
    private readonly IApplicationClientService _appCltService;

    public ApplicationCltGetRAGConfigQueryHandler(IClientContext clientContext, IApplicationClientService appCltService)
    {
        _clientContext = clientContext;
        _appCltService = appCltService;
    }

    public async Task<Result<RAGConfigDto?>> Handle(ApplicationCltGetRAGConfigQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_clientContext.ClientId) || string.IsNullOrWhiteSpace(_clientContext.Tenant))
            return Result.Failure<RAGConfigDto?>(new("Unauthorized", "No tiene autorización para esta acción"));

        var foundRslt = await _appCltService.GetRAGConfig(new Guid(_clientContext.ClientId), _clientContext.Tenant, cancellationToken);

        return foundRslt.IsFailure
            ? Result.Failure<RAGConfigDto?>(foundRslt.Error)
            : foundRslt.Value == null
                ? null
                : new RAGConfigDto(
                    foundRslt.Value.Prompt,
                    foundRslt.Value.TargetTokens,
                    foundRslt.Value.MaxTokens,
                    foundRslt.Value.MinTokens,
                    foundRslt.Value.OverlapTokens);
    }
}
