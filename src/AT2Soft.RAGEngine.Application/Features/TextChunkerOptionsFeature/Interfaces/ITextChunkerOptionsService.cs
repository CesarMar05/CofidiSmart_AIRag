using System;
using AT2Soft.Application.Result;
using AT2Soft.RAGEngine.Application.Abstractions.TextChunker;

namespace AT2Soft.RAGEngine.Application.Features.TextChunkerOptionsFeature.Interfaces;

public interface ITextChunkerOptionsService
{
    Task<TextChunkerOptions> GetTextChunkerOptions(CancellationToken cancellationToken = default);
}
