using AT2Soft.RAGEngine.Application.Features.Authentication.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AT2Soft.RAGEngine.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> GetToken(GetTokenCommand command, CancellationToken cancellationToken)
    {
        var rslt = await _mediator.Send(command, cancellationToken);

        return rslt.IsSuccess
            ? Ok(rslt.Value)
            : rslt.Error.Code == "Unauthorized"
                ? Unauthorized()
                : BadRequest(rslt.Error.Code);
    }
}
