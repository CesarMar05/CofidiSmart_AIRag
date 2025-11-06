using AT2Soft.RAGEngine.Application.Abstractions.Features.AppClient.DTOs;
using AT2Soft.RAGEngine.Application.Features.AppClient.Commands;
using AT2Soft.RAGEngine.Application.Features.AppClient.Queries;
using AT2Soft.RAGEngine.WebAPI.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AT2Soft.RAGEngine.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ApplicationClientController : ControllerBase
{
    private readonly IMediator _mediator;

    public ApplicationClientController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Policy = ScopePolicies.Admin)]
    [HttpPost]
    public async Task<IActionResult> ApplicationAdd(AppClientAddCommand command, CancellationToken cancellationToken)
    {
        var rslt = await _mediator.Send(command, cancellationToken);

        return rslt.IsSuccess
            ? Ok(rslt.Value)
            : BadRequest(rslt.Error);
    }

    [Authorize(Policy = ScopePolicies.Admin)]
    [HttpGet]
    public async Task<IActionResult> ApplicationGetList(CancellationToken cancellationToken)
    {
        var query = new ApplicationCltGetListQuery();
        var rslt = await _mediator.Send(query, cancellationToken);

        return rslt.IsSuccess
            ? Ok(rslt.Value)
            : BadRequest(rslt.Error);
    }

    [Authorize(Policy = ScopePolicies.Admin)]
    [HttpGet("{applicationId}")]
    public async Task<IActionResult> ApplicationGetElement(Guid applicationId, [FromQuery] bool fulldata, CancellationToken cancellationToken)
    {
        var query = new ApplicationCltGetByIdQuery(applicationId, fulldata);
        var rslt = await _mediator.Send(query, cancellationToken);

        return rslt.IsSuccess
            ? Ok(rslt.Value)
            : BadRequest(rslt.Error);
    }

    [Authorize(Policy = ScopePolicies.Admin)]
    [HttpPost("{applicationId}/ragconfig")]
    public async Task<IActionResult> ApplicationSetPrompt(Guid applicationId, [FromBody] RAGConfigDto request, CancellationToken cancellationToken)
    {
        var query = new ApplicationCltSetRAGConfigCommand(applicationId, string.Empty, request.Prompt, request.TargetTokens, request.MaxTokens, request.MinTokens, request.OverlapTokens);
        var rslt = await _mediator.Send(query, cancellationToken);

        return rslt.IsSuccess
            ? Ok()
            : rslt.Error.Code.Contains("NotFound", StringComparison.InvariantCultureIgnoreCase)
                ? NotFound($"No se localizó el ApplicacionClient con Id {applicationId}")
                : BadRequest(rslt.Error);
    }

    [Authorize(Policy = ScopePolicies.Admin)]
    [HttpPost("{applicationId}/{tenant}/ragconfig")]
    public async Task<IActionResult> ApplicationSetPrompt(Guid applicationId,  string tenant, [FromBody] RAGConfigDto request, CancellationToken cancellationToken)
    {
        var query = new ApplicationCltSetRAGConfigCommand(applicationId, tenant, request.Prompt, request.TargetTokens, request.MaxTokens, request.MinTokens, request.OverlapTokens);
        var rslt = await _mediator.Send(query, cancellationToken);

        return rslt.IsSuccess
            ? Ok()
            : rslt.Error.Code.Contains("NotFound", StringComparison.InvariantCultureIgnoreCase)
                ? NotFound($"No se localizó el ApplicacionClient con Id {applicationId}")
                : BadRequest(rslt.Error);
    }
}