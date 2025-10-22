using AT2Soft.RAGEngine.Application.Abstractions.Authentication;
using AT2Soft.RAGEngine.Application.Abstractions.WebApiDTOs;
using AT2Soft.RAGEngine.Application.Features.Chat.Commands;
using AT2Soft.RAGEngine.Domain.Interfaces.Services;
using AT2Soft.RAGEngine.WebAPI.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AT2Soft.RAGEngine.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAIModelService _ollamaService;
    private readonly IClientContext _clientContext;

    public ChatController(IAIModelService ollamaService, IMediator mediator, IClientContext clientContext)
    {
        _ollamaService = ollamaService;
        _mediator = mediator;
        _clientContext = clientContext;
    }

    [Authorize(Policy = ScopePolicies.Query)]
    [HttpPost("{tenant}")]
    public async Task<IActionResult> Ask(string tenant,[FromBody] ChatAskRequest request, CancellationToken cancellationToken)
    {
        var command = new ChatAskCommand(_clientContext.ClientId, tenant, request.Question, request.SearchContext);
        var rslt = await _mediator.Send(command, cancellationToken);

        return rslt.IsSuccess
            ? Ok(rslt.Value)
            : BadRequest(rslt.Error);
    }
}