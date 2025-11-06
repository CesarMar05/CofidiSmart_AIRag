using AT2Soft.Application.Result;
using AT2Soft.RAGEngine.Application.DTOs;
using AT2Soft.RAGEngine.Application.Interfaces.Security;
using AT2Soft.RAGEngine.Application.Persistence.Interfaces;
using MediatR;

namespace AT2Soft.RAGEngine.Application.Features.Authentication.Commands;

public sealed record GetTokenCommand(Guid AppId, string AppKey, string UserId, string Tenant, IReadOnlyList<string>? Divisions, string? Scope) : IRequest<Result<TokenResponse>>;

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
        if (string.IsNullOrWhiteSpace(request.Tenant) || string.IsNullOrWhiteSpace(request.UserId))
            return Result.Failure<TokenResponse>(new("Unauthorized", "UserId y Tenant deben ser incluidos"));

        var client = await _applicationClientRepository.GetByIdAsync(request.AppId, cancellationToken);
        if (client is null || !client.IsActive || !_secretHasherService.Verify(request.AppKey, client.ClientSecretHash))
            return Result.Failure<TokenResponse>(new("Unauthorized", "Unauthorized"));
        
        var requested = (request.Scope ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
        if (requested.Length == 0)
            requested = client.AllowedScopes.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var allowed = client.AllowedScopes.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (!requested.All(s => allowed.Contains(s)))
            return Result.Failure<TokenResponse>(new("InvalidScope", "InvalidScope"));

        return _jwtService.GenerateToken(client.ApplicationClientId, request.UserId, request.Tenant, request.Divisions ?? [], string.Join(' ', requested));
    }
}