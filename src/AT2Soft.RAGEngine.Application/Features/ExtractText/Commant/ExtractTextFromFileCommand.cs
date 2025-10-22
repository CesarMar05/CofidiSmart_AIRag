using System;
using AT2Soft.Application.Result;
using AT2Soft.RAGEngine.Application.Abstractions.TextExtractor;
using MediatR;

namespace AT2Soft.RAGEngine.Application.Features.ExtractText.Commant;

public sealed record ExtractTextFromFileCommand(string FileName, Stream FileStream) : IRequest<Result<TextExtractorResponse>>;

internal sealed class ExtractTextFromFileCommandHandler : IRequestHandler<ExtractTextFromFileCommand, Result<TextExtractorResponse>>
{
    private readonly IFileTextExtractorFactory _fileTextExtractorFactory;

    public ExtractTextFromFileCommandHandler(IFileTextExtractorFactory fileTextExtractorFactory)
    {
        _fileTextExtractorFactory = fileTextExtractorFactory;
    }

    public async Task<Result<TextExtractorResponse>> Handle(ExtractTextFromFileCommand request, CancellationToken cancellationToken)
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        try
        {
            var ext = Path.GetExtension(request.FileName);

            var extractor = _fileTextExtractorFactory.GetByExtension(ext);
            var text = await extractor.ExtractTextAsync(request.FileStream, cancellationToken);

            sw.Stop();

            return new TextExtractorResponse(request.FileName, text.Length, sw.Elapsed, text);
        }
        catch (NotSupportedException ex)
        {
            sw.Stop();

            return Result.Failure<TextExtractorResponse>(new("FileTextExtractorNotFound", ex.Message));
        }
        catch (Exception ex)
        {
            sw.Stop(); 

            return Result.Failure<TextExtractorResponse>(new("FileTextExtractorFail", ex.Message));
        }

    }
}
