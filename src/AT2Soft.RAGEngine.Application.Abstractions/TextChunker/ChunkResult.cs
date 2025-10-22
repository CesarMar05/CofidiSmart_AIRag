using System;

namespace AT2Soft.RAGEngine.Application.Abstractions.TextChunker;

public record ChunkResult(int Index, string Text, int EstimatedTokens);
