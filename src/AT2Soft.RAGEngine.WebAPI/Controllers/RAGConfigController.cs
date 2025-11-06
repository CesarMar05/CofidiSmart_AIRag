using AT2Soft.RAGEngine.Application.Abstractions.Authentication;
using AT2Soft.RAGEngine.Application.Abstractions.Features.AppClient.DTOs;
using AT2Soft.RAGEngine.Application.Features.AppClient.Commands;
using AT2Soft.RAGEngine.Application.Features.AppClient.Queries;
using AT2Soft.RAGEngine.WebAPI.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AT2Soft.RAGEngine.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RAGConfigController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IClientContext _clientContext;

    public RAGConfigController(IMediator mediator, IClientContext clientContext)
    {
        _mediator = mediator;
        _clientContext = clientContext;
    }

    [Authorize(Policy = ScopePolicies.Ingest)]
    [HttpPost]
    public async Task<IActionResult> ApplicationSetPrompt([FromBody] RAGConfigDto request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_clientContext.ClientId) || string.IsNullOrWhiteSpace(_clientContext.Tenant))
            return Unauthorized();

        var query = new ApplicationCltSetRAGConfigCommand(new Guid(_clientContext.ClientId), _clientContext.Tenant, request.Prompt, request.TargetTokens, request.MaxTokens, request.MinTokens, request.OverlapTokens);
        var rslt = await _mediator.Send(query, cancellationToken);

        return rslt.IsSuccess
            ? Ok()
            : rslt.Error.Code.Contains("NotFound", StringComparison.InvariantCultureIgnoreCase)
                ? NotFound($"No se localizó el ApplicacionClient con Id {_clientContext.ClientId}")
                : BadRequest(rslt.Error);
    }

    [Authorize(Policy = ScopePolicies.Ingest)]
    [HttpGet]
    public async Task<IActionResult> ApplicationGetPrompt(CancellationToken cancellationToken)
    {
        var query = new ApplicationCltGetRAGConfigQuery();
        var rslt = await _mediator.Send(query, cancellationToken);

        return rslt.IsSuccess
            ? Ok(rslt.Value)
            : rslt.Error.Code.Contains("Unauthorized", StringComparison.InvariantCultureIgnoreCase)
                ? Unauthorized()
                : BadRequest(rslt.Error);        
    }
}