namespace AT2Soft.RAGEngine.Application.Abstractions.TextExtractor;

public interface IFileTextExtractor
{
    /// <summary>Extensiones que este extractor soporta, ej: ".txt", ".pdf"</summary>
    IReadOnlyCollection<string> SupportedExtensions { get; }

    /// <summary>Extrae texto de un stream de archivo.</summary>
    Task<string> ExtractTextAsync(Stream fileStream, CancellationToken cancellationToken = default);
}
