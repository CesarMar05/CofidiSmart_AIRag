using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace AT2Soft.RAGEngine.Application.Helpers;

public partial class HttpHelper
{
    public static async Task<string> UrlGetContentAsyn(string url, CancellationToken cancellationToken = default)
    {
        var httpClient = new HttpClient();

        var html = await httpClient.GetStringAsync(url, cancellationToken);

        return html;
    }

    public static async Task<string> TextExtractorAsync(string url, CancellationToken cancellationToken = default)
    {
        using var httpClient = new HttpClient();

        var rslt = await httpClient.GetAsync(url, cancellationToken);
        //var html = await httpClient.GetStringAsync(url, cancellationToken);

        if (!rslt.IsSuccessStatusCode)
            return string.Empty;

        var html = await rslt.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(html))
            return string.Empty;

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        // Elimina <script>, <style>, <noscript>, <head>
        string[] removableTags = ["script", "style", "noscript", "head"];

        foreach (var tag in removableTags)
        {
            try
            {
                foreach (var node in doc.DocumentNode.SelectNodes($"//{tag}"))
                {
                    node.Remove();
                }
            }
            catch
            {
                continue;
            }
        }

        var rawText = doc.DocumentNode.InnerText;

        // Limpia espacios múltiples, tabs, saltos de línea innecesarios
        var cleanText = MyRegex().Replace(rawText, " ").Trim();

        return cleanText;
    }

    public static async Task<string> GetCleanTextFromUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        using var httpClient = new HttpClient();
        var html = await httpClient.GetStringAsync(url, cancellationToken);

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        // Eliminar <script> y <style>
        var scriptNodes = doc.DocumentNode.SelectNodes("//script|//style");
        if (scriptNodes != null)
        {
            foreach (var node in scriptNodes)
            {
                node.Remove();
            }
        }

        // Extraer texto crudo
        var rawText = doc.DocumentNode.InnerText;

        // Limpiar:
        // 1. Quitar múltiples espacios y tabs
        var text = Regex.Replace(rawText, @"\s+", " ");

        // 2. Quitar caracteres no imprimibles
        text = Regex.Replace(text, @"[^\u0020-\u007E\u00A0-\u00FF]", string.Empty);

        // 3. Trim para dejar limpio inicio/fin
        text = text.Trim();

        return text;
    }

    [System.Text.RegularExpressions.GeneratedRegex(@"\s{2,}")]
    private static partial System.Text.RegularExpressions.Regex MyRegex();
}