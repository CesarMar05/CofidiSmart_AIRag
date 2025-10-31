using System.Diagnostics;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;

namespace AT2Soft.RAGEngine.Infrastructure.AIModel.DelegatingHandlers;

public sealed class HttpLoggingHandler : DelegatingHandler
{
    private readonly ILogger<HttpLoggingHandler> _logger;
    private readonly HashSet<string> _redactHeaders = new(StringComparer.OrdinalIgnoreCase)
    {
        "Authorization", "X-Api-Key", "ApiKey", "X-Client-Secret"
    };

    // Tamaños máximos para evitar logs gigantes
    private const int MaxBodyChars = 8_000;

    public HttpLoggingHandler(ILogger<HttpLoggingHandler> logger) => _logger = logger;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        var correlationId = Activity.Current?.Id ?? Guid.NewGuid().ToString("N");

        // ---- REQUEST ----
        string? requestBody = null;
        if (request.Content is not null)
        {
            // Ojo: si tu contenido es un stream no reenviable, usa Buffering (HttpClientFactory ya bufferiza por defecto en la mayoría de casos)
            requestBody = await request.Content.ReadAsStringAsync(ct);
            if (requestBody.Length > MaxBodyChars)
                requestBody = requestBody[..MaxBodyChars] + $"... (truncated {requestBody.Length - MaxBodyChars} chars)";
        }

        _logger.LogInformation(
            "HTTP OUT → {Method} {Url} | CorrelationId={CorrelationId} | Headers={Headers} | Body={Body}",
            request.Method, request.RequestUri, correlationId, Redact(request.Headers), requestBody
        );

        HttpResponseMessage? response = null;
        string? responseBody = null;
        try
        {
            response = await base.SendAsync(request, ct);

            if (response.Content is not null)
            {
                responseBody = await response.Content.ReadAsStringAsync(ct);
                if (responseBody.Length > MaxBodyChars)
                    responseBody = responseBody[..MaxBodyChars] + $"... (truncated {responseBody.Length - MaxBodyChars} chars)";
            }

            sw.Stop();
            _logger.LogInformation(
                "HTTP IN  ← {StatusCode} ({ElapsedMs} ms) {Method} {Url} | CorrelationId={CorrelationId} | Headers={Headers} | Body={Body}",
                (int)response.StatusCode, sw.ElapsedMilliseconds, request.Method, request.RequestUri, correlationId, Redact(response.Headers), responseBody
            );

            return response;
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex,
                "HTTP FAIL ← ({ElapsedMs} ms) {Method} {Url} | CorrelationId={CorrelationId}",
                sw.ElapsedMilliseconds, request.Method, request.RequestUri, correlationId);
            throw;
        }
    }

    private Dictionary<string, string> Redact(HttpHeaders headers)
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var h in headers)
        {
            var value = string.Join(";", h.Value ?? Array.Empty<string>());
            dict[h.Key] = _redactHeaders.Contains(h.Key) ? "***REDACTED***" : value;
        }
        return dict;
    }
}