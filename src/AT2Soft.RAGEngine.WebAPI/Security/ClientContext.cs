using AT2Soft.RAGEngine.Application.Abstractions.Authentication;

namespace AT2Soft.RAGEngine.WebAPI.Security;

public class ClientContext : IClientContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClientContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid ClientId =>
        _httpContextAccessor.HttpContext?.User?.FindFirst("client_id") == null
        ? new Guid()
        : new Guid(_httpContextAccessor.HttpContext?.User?.FindFirst("client_id")?.Value!);
}
