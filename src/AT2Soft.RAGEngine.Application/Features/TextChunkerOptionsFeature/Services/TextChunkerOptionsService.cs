using AT2Soft.RAGEngine.Application.Abstractions.Authentication;
using AT2Soft.RAGEngine.Application.Abstractions.TextChunker;
using AT2Soft.RAGEngine.Application.Features.TextChunkerOptionsFeature.Interfaces;
using AT2Soft.RAGEngine.Application.Persistence.Interfaces;

namespace AT2Soft.RAGEngine.Application.Features.TextChunkerOptionsFeature.Services;

public class TextChunkerOptionsService : ITextChunkerOptionsService
{
    private readonly IApplicationClientRepository _applicationClientRepository;
    private readonly IClientContext _clientContext;

    public TextChunkerOptionsService(IApplicationClientRepository applicationClientRepository, IClientContext clientContext)
    {
        _applicationClientRepository = applicationClientRepository;
        _clientContext = clientContext;
    }

    public async Task<TextChunkerOptions> GetTextChunkerOptions(CancellationToken cancellationToken = default)
    {
        var tco = new TextChunkerOptions();

        if (string.IsNullOrWhiteSpace(_clientContext.ClientId) || string.IsNullOrWhiteSpace(_clientContext.Tenant))
            return tco;
        
        try
        {
            var aprc = await _applicationClientRepository.GetApplicationClientRAGConfig(new Guid(_clientContext.ClientId), _clientContext.Tenant, cancellationToken);
            if (aprc == null)
                return tco;

            return new TextChunkerOptions
            {
                TargetTokens = aprc.TargetTokens,
                MaxTokens = aprc.MaxTokens,
                MinTokens = aprc.MinTokens,
                OverlapTokens = aprc.OverlapTokens
            };
        }
        catch
        {
            return tco;
        }
    }
}
