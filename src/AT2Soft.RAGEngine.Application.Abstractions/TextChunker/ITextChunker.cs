using System;

namespace AT2Soft.RAGEngine.Application.Abstractions.TextChunker;

public interface ITextChunkerService
{
    List<ChunkResult> Chunk(string text, TextChunkerOptions opt);
}
