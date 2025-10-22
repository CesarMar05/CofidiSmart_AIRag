using AT2Soft.Application.Result;
using AT2Soft.RAGEngine.Application.DTOs;
using AT2Soft.RAGEngine.Application.Interfaces.Repositories;
using AT2Soft.RAGEngine.Application.Interfaces.Security;
using MediatR;

namespace AT2Soft.RAGEngine.Application.Features.Authentication.Commands;

public sealed record GetTokenCommand(Guid AppId, string AppKey, string? Scope) : IRequest<Result<TokenResponse>>;

internal sealed class GetTokenCommandHandler : IRequestHandler<GetTokenCommand, Result<TokenResponse>>
{
    private readonly IApplicationClientRepository _applicationClientRepository;
    private readonly ISecretHasherService _secretHasherService;
    private readonly IJwtService _jwtService;

    public GetTokenCommandHandler(IApplicationClientRepository applicationClientRepository, ISecretHasherService secretHasherService, IJwtService jwtService)
    {
        _applicationClientRepository = applicationClientRepository;
        _secretHasherService = secretHasherService;
        _jwtService = jwtService;
    }

    public async Task<Result<TokenResponse>> Handle(GetTokenCommand request, CancellationToken cancellationToken)
    {
        var client = await _applicationClientRepository.GetByIdAsync(request.AppId, cancellationToken);
        if (client is null || !client.IsActive || !_secretHasherService.Verify(request.AppKey, client.ClientSecretHash))
            return Result.Failure<TokenResponse>(new("Unauthorized", "Unauthorized"));

        var requested = (request.Scope ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
        if (requested.Length == 0)
            requested = client.AllowedScopes.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var allowed = client.AllowedScopes.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (!requested.All(s => allowed.Contains(s)))
            return Result.Failure<TokenResponse>(new("InvalidScope", "InvalidScope"));

        return _jwtService.GenerateToken(client.ApplicationClientId, string.Join(' ', requested));
    }
}