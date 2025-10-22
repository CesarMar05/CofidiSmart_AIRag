using AT2Soft.RAGEngine.Application.Abstractions.TextExtractor;

namespace AT2Soft.RAGEngine.Infrastructure.TextExtractors;

public class TxtTextExtractor: IFileTextExtractor
{
    public IReadOnlyCollection<string> SupportedExtensions => [".txt"];

    public async Task<string> ExtractTextAsync(Stream fileStream, CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(fileStream, leaveOpen: true);
        var text = await reader.ReadToEndAsync(cancellationToken);
        
        return text;
    }
}