using System;

namespace AT2Soft.RAGEngine.Application.Abstractions.TextChunker;

public sealed record TextChunkerOptions(
    int TargetTokens = 350,
    int OverlapTokens = 60,
    int MaxTokens = 450,
    int MinTokens = 150
);
