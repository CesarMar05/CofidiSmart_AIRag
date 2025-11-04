
namespace AT2Soft.RAGEngine.Application.Abstractions.TextChunker;

public sealed record TextChunkerOptionsOld(
    int TargetTokens = 350,
    int OverlapTokens = 60,
    int MaxTokens = 450,
    int MinTokens = 150
);

public sealed class TextChunkerOptions
{
    public int TargetTokens { get; init; } = 350;   // tamaño preferido
    public int MaxTokens { get; init; } = 450;      // límite duro
    public int MinTokens { get; init; } = 150;
    public int OverlapTokens { get; init; } = 60;   // solape
    public int MaxChars => MaxTokens * 4;           // fallback por caracteres
}
