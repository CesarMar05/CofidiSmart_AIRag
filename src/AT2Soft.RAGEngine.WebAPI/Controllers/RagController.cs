using AT2Soft.RAGEngine.Application.Abstractions.KnowledgeDocument;
using AT2Soft.RAGEngine.Application.Abstractions.TextChunker;
using AT2Soft.RAGEngine.Application.Features.KnowledgeDocument.Commands;
using AT2Soft.RAGEngine.Application.Features.TextChunkerOptionsFeature.Interfaces;
using AT2Soft.RAGEngine.Application.Interfaces.Services;
using AT2Soft.RAGEngine.WebAPI.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AT2Soft.RAGEngine.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RagController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IRagIngestJobServices _ragIngestJobServices;
    private readonly ITextChunkerOptionsService _textChunkerOptionsSercvice;

    public RagController(IMediator mediator, IRagIngestJobServices ragIngestJobServices, ITextChunkerOptionsService textChunkerOptionsSercvice)
    {
        _mediator = mediator;
        _ragIngestJobServices = ragIngestJobServices;
        _textChunkerOptionsSercvice = textChunkerOptionsSercvice;
    }

    [Authorize(Policy = ScopePolicies.Ingest)]
    [HttpPost("ingest/text")]
    public async Task<IActionResult> Upload([FromBody] KDReceiveTextRequest request, CancellationToken cancellationToken)
    {
        var tco = request.TextChunkerOptions
            ?? await _textChunkerOptionsSercvice.GetTextChunkerOptions(cancellationToken);
            
        var command = new KDReceiveTextCommand(request.Metadata, tco, request.Text);
        var rslt = await _mediator.Send(command, cancellationToken);

        return rslt.IsSuccess
            ? AcceptedAtAction(nameof(GetJob), new { tenantid = request.Metadata, jobId = rslt.Value }, new { jobId = rslt.Value })
            : rslt.Error.Code.Contains("Unauthorized")
                ? Unauthorized(rslt.Error)
                : BadRequest(rslt.Error);
    }

    [DisableRequestSizeLimit]
    [RequestFormLimits(MultipartBodyLengthLimit = 200_000_000)]
    [Authorize(Policy = ScopePolicies.Ingest)]
    [HttpPost("ingest/file")]
    public async Task<IActionResult> UploadFile([FromBody] KDReceiveFileRequest request, CancellationToken cancellationToken)
    {
        if (request.File is null || request.File.FileContent == null || request.File.FileContent.Length == 0 || request.File.FileName == null || request.File.FileName.Length <= 3) return BadRequest("Archivo vacío.");
        
        var tco = request.TextChunkerOptions
            ?? await _textChunkerOptionsSercvice.GetTextChunkerOptions(cancellationToken);

        using var stream = new MemoryStream(request.File.FileContent);

        var cmd = new KDReceiveFileCommand(request.Metadata, tco, request.File.FileName, stream);
        var rslt = await _mediator.Send(cmd, cancellationToken);

        return rslt.IsSuccess
            ? AcceptedAtAction(nameof(GetJob), new { tenantid = request.Metadata.TenantId, jobId = rslt.Value }, new { jobId = rslt.Value })
            : rslt.Error.Code.Contains("Unauthorized")
                ? Unauthorized()
                : BadRequest(rslt.Error);
    }

    [Authorize(Policy = ScopePolicies.Ingest)]
    [HttpPost("ingest/url")]
    public async Task<IActionResult> UploadUrl([FromBody] KDReceiveUrlRequest request, CancellationToken cancellationToken)
    {
        var tco = request.TextChunkerOptions
            ?? await _textChunkerOptionsSercvice.GetTextChunkerOptions(cancellationToken);

        var command = new KDReceiveUrlCommand(request.Metadata, tco, request.Url);
        var rslt = await _mediator.Send(command, cancellationToken);

        return rslt.IsSuccess
            ? AcceptedAtAction(nameof(GetJob), new { tenantid = request.Metadata.TenantId, jobId = rslt.Value }, new { jobId = rslt.Value })
            : rslt.Error.Code.Contains("Unauthorized")
                ? Unauthorized()
                : BadRequest(rslt.Error);
    }

    [Authorize(Policy = ScopePolicies.Ingest)]
    [HttpGet("jobs")]
    public async Task<IActionResult> GetJobList(CancellationToken cancellationToken)
    {
        var jobRslt = await _ragIngestJobServices.GetRagIngestJobInfoList(cancellationToken);
        return jobRslt.IsFailure
            ? jobRslt.Error.Code.Contains("NotFound")
                ? NotFound()
                : jobRslt.Error.Code.Contains("Unauthorized")
                    ? Unauthorized()
                    : BadRequest(jobRslt.Error)
            : Ok(jobRslt.Value);
    }

    [Authorize(Policy = ScopePolicies.Ingest)]
    [HttpGet("jobs/{jobId:guid}")]
    public async Task<IActionResult> GetJob(Guid jobId, CancellationToken cancellationToken)
    {
        var jobRslt = await _ragIngestJobServices.GetRagIngestJobInfo(jobId, cancellationToken);
        return jobRslt.IsFailure
            ? jobRslt.Error.Code.Contains("NotFound")
                ? NotFound()
                : jobRslt.Error.Code.Contains("Unauthorized")
                    ? Unauthorized()
                    : BadRequest(jobRslt.Error)
            : Ok(jobRslt.Value);
    }
}
