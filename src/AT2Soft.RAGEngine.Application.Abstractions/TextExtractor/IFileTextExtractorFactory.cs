using System;

namespace AT2Soft.RAGEngine.Application.Abstractions.TextExtractor;

public interface IFileTextExtractorFactory
{
    /// <summary>Obtiene el extractor adecuado para una extensión (con o sin punto).</summary>
    IFileTextExtractor GetByExtension(string extension);
}
