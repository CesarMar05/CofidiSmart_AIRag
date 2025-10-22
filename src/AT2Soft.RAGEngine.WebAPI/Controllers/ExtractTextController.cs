using AT2Soft.RAGEngine.Application.Abstractions.TextExtractor;
using AT2Soft.RAGEngine.Application.Features.ExtractText.Commant;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AT2Soft.RAGEngine.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExtractTextController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExtractTextController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("url")]
    public async Task<IActionResult> FromUrl([FromBody] TextExtractorUrlRequest request, CancellationToken cancellationToken)
    {
        var command = new ExtractTextFromUrlCommand(request.Url);
        var rslt = await _mediator.Send(command, cancellationToken);

        return rslt.IsSuccess
            ? Ok(rslt.Value)
            : BadRequest(rslt.Error);
    }

    [HttpPost("file")]
    public async Task<IActionResult> FromFile([FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0) return BadRequest("Archivo vacío.");
        await using var stream = file.OpenReadStream();

        var command = new ExtractTextFromFileCommand(file.FileName, stream);
        var rslt = await _mediator.Send(command, cancellationToken);

        return rslt.IsSuccess
            ? Ok(rslt.Value)
            : BadRequest(rslt.Error);
    }
}