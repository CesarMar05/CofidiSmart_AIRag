using System;

namespace AT2Soft.RAGEngine.Application.Features.Prompt.Interfaces;

public interface IPromptServices
{
    Task<string> GetPrompt(Guid applicationId, string query, List<string> contextChunks, CancellationToken cancellationToken = default);
}
