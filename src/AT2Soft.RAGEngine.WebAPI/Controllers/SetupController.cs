using AT2Soft.RAGEngine.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace AT2Soft.RAGEngine.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SetupController(IApplicationClientService applicationClientService) : ControllerBase
{
    private readonly IApplicationClientService _applicationClientService = applicationClientService;

    [HttpPost]
    public async Task<IActionResult> Initialize([FromQuery] string name, CancellationToken cancellationToken)
    {
        var remoteIp = HttpContext.Connection.RemoteIpAddress;
        if (remoteIp == null || !(remoteIp.ToString() == "127.0.0.1" || remoteIp.ToString() == "::1"))
            return Unauthorized("Este endpoint solo puede ser consumido desde localhost");

        var existAdminRslt = await _applicationClientService.ExistAdmin(cancellationToken);
        if (existAdminRslt.IsFailure || existAdminRslt.Value)
            return BadRequest("Ya existe la aplicación de Administración");

        if (name == null || name.Length < 3 || name.Length > 12 || name.Contains(" "))
            return BadRequest("Name debe contenert al menos 3 caracteres, no mas de 12 y no contener espacios en blanco");

        var rslt = await _applicationClientService.AddAppClient(name, "admin ingest query", cancellationToken);
        return rslt.IsSuccess
            ? Ok(rslt.Value)
            : BadRequest(rslt.Error);
    }
}