using AT2Soft.RAGEngine.Application.Abstractions.Authentication;
using AT2Soft.RAGEngine.Application.Abstractions.KnowledgeDocument;
using AT2Soft.RAGEngine.Application.Abstractions.TextChunker;
using AT2Soft.RAGEngine.Application.Features.KnowledgeDocument.Commands;
using AT2Soft.RAGEngine.Application.Features.KnowledgeDocument.Queries;
using AT2Soft.RAGEngine.WebAPI.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AT2Soft.RAGEngine.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class KDocumentController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IClientContext _clientContext;

    public KDocumentController(IMediator mediator, IClientContext clientContext)
    {
        _mediator = mediator;
        _clientContext = clientContext;
    }

    [Authorize(Policy = ScopePolicies.Query)]
    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] KDSearchRequest request, CancellationToken cancellationToken)
    {
        var query = new KDSearchQuery(
            _clientContext.ClientId,
            request.TenantId,
            request.Query,
            request.TopK
        );
        var rslt = await _mediator.Send(query, cancellationToken);

        return rslt.IsSuccess
            ? Ok(rslt.Value)
            : BadRequest(rslt.Error);
    }

    [Authorize(Policy = ScopePolicies.Query)]
    [HttpGet("{tenant}")]
    public async Task<IActionResult> GetList(string tenant, [FromQuery] int take, CancellationToken cancellationToken)
    {
        var query = new KDGetListQuery(_clientContext.ClientId, tenant, take);
        var rslt = await _mediator.Send(query, cancellationToken);

        return rslt.IsSuccess
            ? Ok(rslt.Value)
            : BadRequest(rslt.Error);
    }

    [Authorize(Policy = ScopePolicies.Query)]
    [HttpGet("{tenant}/{documentid}")]
    public async Task<IActionResult> GetDocument(string tenant, Guid documentid, [FromQuery]bool fulldata, CancellationToken cancellationToken)
    {
        var query = new KDGetByIdQuery(_clientContext.ClientId, tenant, documentid, fulldata);
        var rslt = await _mediator.Send(query, cancellationToken);

        return rslt.IsSuccess
            ? Ok(rslt.Value)
            : BadRequest(rslt.Error);
    }
}