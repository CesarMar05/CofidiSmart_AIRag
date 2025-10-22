using UglyToad.PdfPig;
using System.Text.RegularExpressions;

namespace AT2soft.PdfTextExtractor;

public class PdfPigTextExtractor

{
    // Opcional: parámetros para limpiar texto
    private static readonly Regex MultiSpace = new(@"\s{2,}", RegexOptions.Compiled);
    private static readonly Regex HyphenBreak = new(@"-\r?\n", RegexOptions.Compiled); // “palabra-\ncontinuación”

    public Task<IReadOnlyList<PageText>> ExtractAsync(Stream pdfContent, CancellationToken ct = default)
    {
        var pages = new List<PageText>();

        using var doc = PdfDocument.Open(pdfContent);
        foreach (var page in doc.GetPages())
        {
            ct.ThrowIfCancellationRequested();
            // PdfPig ya da el “contenido lógico” de la página:
            var raw = page.Text;

            var cleaned = CleanText(raw);
            pages.Add(new PageText(page.Number, cleaned));
        }

        return Task.FromResult<IReadOnlyList<PageText>>(pages);
    }

    private static string CleanText(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        // Repara cortes por guion al final de línea: ej. “inter- \n nacional” -> “internacional”
        var s = HyphenBreak.Replace(input, string.Empty);

        // Normaliza saltos de línea (mantén párrafos, compacta restos)
        s = s.Replace("\r\n", "\n");

        // Opcional: colapsa espacios múltiple manteniendo saltos de párrafo
        s = string.Join("\n",
            s.Split('\n')
             .Select(line => MultiSpace.Replace(line.Trim(), " "))
        );

        return s.Trim();
    }
}