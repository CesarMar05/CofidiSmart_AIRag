using AT2Soft.RAGEngine.Application.Features.Knowledge.Commands;
using AT2Soft.RAGEngine.Application.Features.Knowledge.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AT2Soft.RAGEngine.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class KnowledgeController : ControllerBase
{
    private readonly IMediator _mediator;

    public KnowledgeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost()]
    public async Task<IActionResult> Add([FromBody] KnowledgeAddCommand command, CancellationToken cancellationToken)
    {
        var rslt = await _mediator.Send(command, cancellationToken);

        return rslt.IsSuccess
            ? Ok()
            : BadRequest(rslt.Error);
    }

    [HttpGet()]
    public async Task<IActionResult> GetList(CancellationToken cancellationToken)
    {
        var query = new KnowledgeGetListQuery();
        var rslt = await _mediator.Send(query, cancellationToken);

        return rslt.IsSuccess
            ? Ok(rslt.Value)
            : BadRequest(rslt.Error);
    }
}