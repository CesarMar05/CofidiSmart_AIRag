using AT2Soft.Application.Result;
using AT2Soft.RAGEngine.Application.DTOs;
using AT2Soft.RAGEngine.Application.Interfaces.Services;
using MediatR;

namespace AT2Soft.RAGEngine.Application.Features.AppClient.Commands;

public sealed record AppClientAddCommand(string ApplicationName) : IRequest<Result<AppClientAddResult>>;

internal class AppClientAddCommandHandler : IRequestHandler<AppClientAddCommand, Result<AppClientAddResult>>
{
    private readonly IApplicationClientService _applicationClientService;

    public AppClientAddCommandHandler(IApplicationClientService applicationClientService)
    {
        _applicationClientService = applicationClientService;
    }

    public async Task<Result<AppClientAddResult>> Handle(AppClientAddCommand request, CancellationToken cancellationToken)
    {
        if (request.ApplicationName == null || request.ApplicationName.Length < 3 || request.ApplicationName.Length > 12 || request.ApplicationName.Contains(' '))
            return Result.Failure<AppClientAddResult>(new("InvlaidApplicationName", $"El nombre de la Aplicación debe contener al menos 3 caracteres, no más de 12 y no contener espacios en blanco"));

        var foundRslt = await _applicationClientService.FindByNameAsync(request.ApplicationName, cancellationToken);
        if (foundRslt.IsFailure)
            return Result.Failure<AppClientAddResult>(foundRslt.Error);

        if (foundRslt.Value != null)
            return Result.Failure<AppClientAddResult>(new("ApplicationClientExist", $"Ya existe una Aplicación con Nombre {request.ApplicationName}"));

        return await _applicationClientService.AddAppClient(request.ApplicationName, "ingest query", cancellationToken);
    }
}
