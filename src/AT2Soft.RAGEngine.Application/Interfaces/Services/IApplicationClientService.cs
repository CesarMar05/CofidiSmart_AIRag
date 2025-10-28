using AT2Soft.Application.Result;
using AT2Soft.RAGEngine.Application.Abstractions.DTOs;
using AT2Soft.RAGEngine.Application.DTOs;
using AT2Soft.RAGEngine.Domain.Entities;

namespace AT2Soft.RAGEngine.Application.Interfaces.Services;

public interface IApplicationClientService
{
    Task<Result<AppClientAddResult>> AddAppClient(string name, string scope, CancellationToken cancellationToken = default);
    Task<Result<ApplicationClientInfo?>> GetAppClient(Guid id, bool fullData, CancellationToken cancellationToken = default);
    Task<Result<ApplicationClientInfo?>> FindByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Result<List<ApplicationClient>>> GetListAppClient(CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistAdmin(CancellationToken cancellationToken = default);
    Task<Result> SetPrompt(Guid applicationClientd, string prompt, CancellationToken cancellationToken = default);
}
