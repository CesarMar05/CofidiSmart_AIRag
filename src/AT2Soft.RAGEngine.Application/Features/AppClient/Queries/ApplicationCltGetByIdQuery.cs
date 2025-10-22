using AT2Soft.Application.Result;
using AT2Soft.RAGEngine.Application.Abstractions.DTOs;
using AT2Soft.RAGEngine.Application.Interfaces.Services;
using MediatR;

namespace AT2Soft.RAGEngine.Application.Features.AppClient.Queries;

public sealed record  ApplicationCltGetByIdQuery(Guid ApplicationClientId, bool FullData) : IRequest<Result<ApplicationClientInfo>>;

internal class ApplicationCltGetByIdQueryHandler : IRequestHandler<ApplicationCltGetByIdQuery, Result<ApplicationClientInfo>>
{
    private readonly IApplicationClientService _applicationClientService;

    public ApplicationCltGetByIdQueryHandler(IApplicationClientService applicationClientService)
    {
        _applicationClientService = applicationClientService;
    }

    public async Task<Result<ApplicationClientInfo>> Handle(ApplicationCltGetByIdQuery request, CancellationToken cancellationToken)
    {
        var findRslt = await _applicationClientService.GetAppClient(request.ApplicationClientId, request.FullData, cancellationToken);
        if (findRslt.IsFailure) return Result.Failure<ApplicationClientInfo>(findRslt.Error);
        if (findRslt.Value == null)
                return Result.Failure<ApplicationClientInfo>(new("ApplicationClientNotFound", $"No se localizó ApplicationClient con Id {request.ApplicationClientId}"));

        return findRslt.Value;
    }
}