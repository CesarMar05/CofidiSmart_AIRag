using AT2Soft.RAGEngine.Application.Abstractions.TextExtractor;

namespace AT2Soft.RAGEngine.Infrastructure.TextExtractors;

public class DocxTextExtractor: IFileTextExtractor
{
    public IReadOnlyCollection<string> SupportedExtensions => [".docx"];

    public Task<string> ExtractTextAsync(Stream fileStream, CancellationToken ct = default)
    {
        // TODO: implementar con librería real
        return Task.FromResult("[DOCX TEXT HERE]");
    }
}