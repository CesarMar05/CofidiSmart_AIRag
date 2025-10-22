using AT2Soft.RAGEngine.Application.Abstractions.Authentication;
using AT2Soft.RAGEngine.Application.Abstractions.KnowledgeDocument;
using AT2Soft.RAGEngine.Application.Abstractions.TextChunker;
using AT2Soft.RAGEngine.Application.Features.KnowledgeDocument.Commands;
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
    private readonly IClientContext _clientContext;
    private readonly IRagIngestJobServices _ragIngestJobServices;

    public RagController(IMediator mediator, IClientContext clientContext, IRagIngestJobServices ragIngestJobServices)
    {
        _mediator = mediator;
        _clientContext = clientContext;
        _ragIngestJobServices = ragIngestJobServices;
    }

    [Authorize(Policy = ScopePolicies.Ingest)]
    [HttpPost("ingest/text")]
    public async Task<IActionResult> Upload([FromBody] KDReceiveTextRequest request, CancellationToken cancellationToken)
    {
        var command = new KDReceiveTextCommand(_clientContext.ClientId, request.Metadata, request.TextChunkerOptions ?? new TextChunkerOptions(), request.Text);
        var rslt = await _mediator.Send(command, cancellationToken);

        return rslt.IsSuccess
            ? AcceptedAtAction(nameof(GetJob), new { tenantid = request.Metadata, jobId = rslt.Value }, new { jobId = rslt.Value })
            : BadRequest(rslt.Error);
    }

    [DisableRequestSizeLimit]
    [RequestFormLimits(MultipartBodyLengthLimit = 200_000_000)]
    [Authorize(Policy = ScopePolicies.Ingest)]
    [HttpPost("ingest/file")]
    public async Task<IActionResult> UploadFile([FromBody] KDReceiveFileRequest request, CancellationToken cancellationToken)
    {
        if (request.File is null || request.File.FileContent == null || request.File.FileContent.Length == 0 || request.File.FileName == null || request.File.FileName.Length <= 3) return BadRequest("Archivo vacío.");

        using var stream = new MemoryStream(request.File.FileContent);

        var cmd = new KDReceiveFileCommand(_clientContext.ClientId, request.Metadata, request.TextChunkerOptions ?? new TextChunkerOptions(), request.File.FileName, stream);
        var rslt = await _mediator.Send(cmd, cancellationToken);

        return rslt.IsSuccess
            ? AcceptedAtAction(nameof(GetJob), new { tenantid = request.Metadata.TenantId, jobId = rslt.Value }, new { jobId = rslt.Value })
            : BadRequest(rslt.Error);
    }

    [Authorize(Policy = ScopePolicies.Ingest)]
    [HttpPost("ingest/url")]
    public async Task<IActionResult> UploadUrl([FromBody] KDReceiveUrlRequest request, CancellationToken cancellationToken)
    {
        var command = new KDReceiveUrlCommand(_clientContext.ClientId, request.Metadata, request.TextChunkerOptions ?? new TextChunkerOptions(), request.Url);
        var rslt = await _mediator.Send(command, cancellationToken);

        return rslt.IsSuccess
            ? AcceptedAtAction(nameof(GetJob), new { tenantid = request.Metadata.TenantId, jobId = rslt.Value }, new { jobId = rslt.Value })
            : BadRequest(rslt.Error);
    }

    [Authorize(Policy = ScopePolicies.Ingest)]
    [HttpGet("jobs/{tenantid}")]
    public async Task<IActionResult> GetJobList( string tenantid, CancellationToken cancellationToken)
    {
        var jobRslt = await _ragIngestJobServices.GetRagIngestJobInfoList(_clientContext.ClientId, tenantid, cancellationToken);
        return jobRslt.IsFailure
            ? jobRslt.Error.Description!.Contains("NotFound")
                ? NotFound()
                : BadRequest(jobRslt.Error)
            : Ok(jobRslt.Value);
    }

    [Authorize(Policy = ScopePolicies.Ingest)]
    [HttpGet("jobs/{tenantid}/{jobId:guid}")]
    public async Task<IActionResult> GetJob(string tenantid, Guid jobId, CancellationToken cancellationToken)
    {
        var jobRslt = await _ragIngestJobServices.GetRagIngestJobInfo(_clientContext.ClientId, tenantid, jobId, cancellationToken);
        return jobRslt.IsFailure
            ? jobRslt.Error.Description!.Contains("NotFound")
                ? NotFound()
                : BadRequest(jobRslt.Error)
            : Ok(jobRslt.Value);
    }
}
