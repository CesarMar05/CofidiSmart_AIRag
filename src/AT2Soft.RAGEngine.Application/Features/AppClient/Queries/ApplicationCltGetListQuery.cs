using AT2Soft.Application.Result;
using AT2Soft.RAGEngine.Application.Interfaces.Services;
using AT2Soft.RAGEngine.Domain.Entities;
using MediatR;

namespace AT2Soft.RAGEngine.Application.Features.AppClient.Queries;

public sealed record ApplicationCltGetListQuery() : IRequest<Result<List<ApplicationClient>>>;

internal class ApplicationCltGetListQueryHandler : IRequestHandler<ApplicationCltGetListQuery, Result<List<ApplicationClient>>>
{
    private readonly IApplicationClientService _applicationClientService;

    public ApplicationCltGetListQueryHandler(IApplicationClientService applicationClientService)
    {
        _applicationClientService = applicationClientService;
    }

    public async Task<Result<List<ApplicationClient>>> Handle(ApplicationCltGetListQuery request, CancellationToken cancellationToken)
    {
        return await _applicationClientService.GetListAppClient(cancellationToken); 
    }
}
