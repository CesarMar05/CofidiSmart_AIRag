using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using Tesseract;

namespace AT2soft.PdfTextExtractor;

public class PdfTextExtractorWithOcr
{
    private readonly string _tessdataPath;

    public PdfTextExtractorWithOcr(string tessdataPath)
    {
        _tessdataPath = tessdataPath;
    }

    public async Task<IReadOnlyList<PageText>> ExtractAsync(Stream pdfContent, CancellationToken ct = default)
    {
        var pages = new List<PageText>();

        using var doc = PdfDocument.Open(pdfContent);
        using var engine = new TesseractEngine(_tessdataPath, "eng", EngineMode.Default);

        foreach (var page in doc.GetPages())
        {
            ct.ThrowIfCancellationRequested();

            // Primero intenta extraer texto digital
            var text = page.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(text))
            {
                // Si no hay texto, hacemos OCR de las imágenes
                foreach (var img in page.GetImages())
                {
                    using var pix = Pix.LoadFromMemory(img.RawBytes.ToArray()); // si tienes las imágenes como bytes
                    using var ocrPage = engine.Process(pix);
                    text += ocrPage.GetText();
                }
            }

            pages.Add(new PageText(page.Number, text.Trim()));
        }

        return await Task.FromResult(pages);
    }
}