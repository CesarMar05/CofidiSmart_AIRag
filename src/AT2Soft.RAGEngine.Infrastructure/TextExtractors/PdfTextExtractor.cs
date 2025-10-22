using System.Text;
using AT2soft.PdfTextExtractor;
using AT2Soft.RAGEngine.Application.Abstractions.TextExtractor;

namespace AT2Soft.RAGEngine.Infrastructure.TextExtractors;

public class PdfTextExtractor: IFileTextExtractor
{
    public IReadOnlyCollection<string> SupportedExtensions => [".pdf"];

    public async Task<string> ExtractTextAsync(Stream fileStream, CancellationToken cancellationToken = default)
    {
        var extractor = new PdfPigTextExtractor();


        var response = new StringBuilder();
        var rslt = await extractor.ExtractAsync(fileStream, cancellationToken);
        foreach (var page in rslt)
            response.AppendLine(page.Text);

            return response.ToString();
    }
}