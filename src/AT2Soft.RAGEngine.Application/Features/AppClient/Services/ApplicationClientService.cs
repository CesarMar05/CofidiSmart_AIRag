using AT2Soft.Application.Result;
using AT2Soft.RAGEngine.Application.Abstractions.DTOs;
using AT2Soft.RAGEngine.Application.DTOs;
using AT2Soft.RAGEngine.Application.Helpers;
using AT2Soft.RAGEngine.Application.Interfaces;
using AT2Soft.RAGEngine.Application.Interfaces.Security;
using AT2Soft.RAGEngine.Application.Interfaces.Services;
using AT2Soft.RAGEngine.Application.Persistence.Interfaces;
using AT2Soft.RAGEngine.Domain.Entities;

namespace AT2Soft.RAGEngine.Application.Features.AppClient.Services;

public class ApplicationClientService : IApplicationClientService
{
    private readonly IApplicationClientRepository _applicationClientRepository;
    private readonly IUnitOfWorkRelational _unitOfWork;
    private readonly ISecretHasherService _secretHasherService;

    public ApplicationClientService(IApplicationClientRepository applicationClientRepository, IUnitOfWorkRelational unitOfWork, ISecretHasherService secretHasherService)
    {
        _applicationClientRepository = applicationClientRepository;
        _unitOfWork = unitOfWork;
        _secretHasherService = secretHasherService;
    }

    public async Task<Result<AppClientAddResult>> AddAppClient(string name, string scope, CancellationToken cancellationToken = default)
    {
        var secret = SecretGenerator.Generate();
        var hash = _secretHasherService.Hash(secret);

        var appClt = new ApplicationClient
        {
            ApplicationClientId = Guid.NewGuid(),
            Name = name,
            IsActive = true,
            AllowedScopes = scope,
            ClientSecretHash = hash
        };

        var rslt = await _applicationClientRepository.AddAsync(appClt, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AppClientAddResult(rslt, secret);
    }

    public async Task<Result<ApplicationClientInfo?>> GetAppClient(Guid id, bool fullData, CancellationToken cancellationToken = default)
    {
        var found = fullData
            ? await _applicationClientRepository.GetFullDataByIdAsync(id, cancellationToken)
            : await _applicationClientRepository.GetByIdAsync(id, cancellationToken);

        return found == null
            ? null
            : (ApplicationClientInfo)found;

    }

    public async Task<Result<ApplicationClientInfo?>> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var found = await _applicationClientRepository.FindByName(name, cancellationToken);
        return found == null
            ? null
            : (ApplicationClientInfo)found;
    }

    public async Task<Result<List<ApplicationClient>>> GetListAppClient(CancellationToken cancellationToken = default)
    {
        return (await _applicationClientRepository.GetListAsync(cancellationToken: cancellationToken))
            .ToList();
    }

    public async Task<Result<bool>> ExistAdmin(CancellationToken cancellationToken = default)
    {
        return await _applicationClientRepository.ExistAdminAsync(cancellationToken);
    }

    public async Task<Result<bool>> ExistByName(CancellationToken cancellationToken = default)
    {
        return await _applicationClientRepository.ExistAdminAsync(cancellationToken);
    }

    public async Task<Result> SetRAGConfig(Guid applicationClientd, string tenant, string prompt, int tokens, int maxTokens, int minTokens, int overlapTokens, CancellationToken cancellationToken = default)
    {
        var found = await _applicationClientRepository.GetByIdAsync(applicationClientd, cancellationToken);
        if (found == null)
            return Result.Failure(new("ApplicationClientNotFound", $"No fue posible localizar Applicationclient"));

        if (string.IsNullOrWhiteSpace(tenant))
        {
            found.Prompt = prompt;
        }

        var acpFound = await _applicationClientRepository.GetApplicationClientRAGConfig(applicationClientd, tenant, cancellationToken);
        acpFound ??= await _applicationClientRepository.AddApplicationClientRAGConfig(new ApplicationClientRAGConfig
        {
            ApplicationClientId = applicationClientd,
            Tenant = tenant,
            Prompt = prompt,
            TargetTokens = tokens,
            MaxTokens = maxTokens,
            MinTokens = minTokens,
            OverlapTokens = overlapTokens
        }, cancellationToken);
        acpFound.Prompt = prompt;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
    
    public async Task<Result<ApplicationClientRAGConfig?>> GetRAGConfig(Guid applicationClientd, string tenant, CancellationToken cancellationToken = default)
    {
        return await _applicationClientRepository.GetApplicationClientRAGConfig(applicationClientd, tenant, cancellationToken);
    }
}
