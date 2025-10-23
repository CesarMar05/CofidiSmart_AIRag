using AT2Soft.RAGEngine.Application.Features.Prompt.Functions;
using AT2Soft.RAGEngine.Application.Features.Prompt.Interfaces;
using AT2Soft.RAGEngine.Application.Persistence.Interfaces;

namespace AT2Soft.RAGEngine.Application.Features.Prompt.Services;

public class PromptServices : IPromptServices
{
    private readonly IApplicationClientRepository _applicationClientRepository;

    public PromptServices(IApplicationClientRepository applicationClientRepository)
    {
        _applicationClientRepository = applicationClientRepository;
    }

    public async Task<string> GetPrompt(Guid applicationId, string query, List<string> contextChunks, CancellationToken cancellationToken = default)
    {
        var context = string.Join("\n", contextChunks);

        var prompt = await _applicationClientRepository.GetOneAsync<string>(
            predicate: ac => ac.ApplicationClientId == applicationId,
            selector: ac => ac.Prompt
        );

        return string.IsNullOrEmpty(prompt)
            ? PromptFunctions.DefaultPrompt(query, context)
            : string.Format(prompt, query, context);
    }
}
