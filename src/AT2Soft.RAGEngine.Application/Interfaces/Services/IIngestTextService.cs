using System;

namespace AT2Soft.RAGEngine.Application.Interfaces.Services;

public interface IIngestTextService
{
    Task ProcessAsync(Guid jobId, CancellationToken ct);
}
