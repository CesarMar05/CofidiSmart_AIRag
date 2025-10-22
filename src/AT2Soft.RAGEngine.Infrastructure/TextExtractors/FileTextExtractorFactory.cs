using AT2Soft.RAGEngine.Application.Abstractions.TextExtractor;

namespace AT2Soft.RAGEngine.Infrastructure.TextExtractors;

public class FileTextExtractorFactory: IFileTextExtractorFactory
{
    private readonly Dictionary<string, IFileTextExtractor> _byExt;

    public FileTextExtractorFactory(IEnumerable<IFileTextExtractor> strategies)
    {
        // Normaliza extensiones a lower y con punto
        _byExt = strategies
            .SelectMany(s => s.SupportedExtensions.Select(ext => (ext: Normalize(ext), strat: s)))
            .GroupBy(x => x.ext)
            .ToDictionary(g => g.Key, g => g.First().strat);
    }

    public IFileTextExtractor GetByExtension(string extension)
    {
        var norm = Normalize(extension);
        if (_byExt.TryGetValue(norm, out var extractor))
            return extractor;

        throw new NotSupportedException($"No hay extractor para la extensión '{norm}'.");
    }

    private static string Normalize(string ext)
    {
        if (string.IsNullOrWhiteSpace(ext)) return string.Empty;
        ext = ext.Trim().ToLowerInvariant();
        return ext.StartsWith('.') ? ext : "." + ext;
    }
}