using AT2Soft.Application.Result;
using AT2Soft.RAGEngine.Application.Helpers;

using MediatR;
using AT2Soft.RAGEngine.Application.Abstractions.TextExtractor;

namespace AT2Soft.RAGEngine.Application.Features.ExtractText.Commant;

public sealed record ExtractTextFromUrlCommand(string Url) : IRequest<Result<TextExtractorResponse>>;

internal sealed class ExtractTextFromUrlCommandHandler : IRequestHandler<ExtractTextFromUrlCommand, Result<TextExtractorResponse>>
{
    public async Task<Result<TextExtractorResponse>> Handle(ExtractTextFromUrlCommand request, CancellationToken cancellationToken)
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        try
        {
            var text = await HttpHelper.GetCleanTextFromUrlAsync(request.Url, cancellationToken);
            sw.Stop();

            return new TextExtractorResponse(request.Url, text.Length, sw.Elapsed, text);
        }
        catch (Exception ex)
        {
            sw.Stop();

            return Result.Failure<TextExtractorResponse>(new("ExtractTextFail", $"Fallo la extreción de Texto de URL. {ex.Message}"));
        }
    }
}
