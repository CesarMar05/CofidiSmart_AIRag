using System.Security.Claims;
using System.Text.Json;
using AT2Soft.RAGEngine.Application.Abstractions.Authentication;

namespace AT2Soft.RAGEngine.WebAPI.Security;

public class ClientContext(IHttpContextAccessor httpContextAccessor) : IClientContext
{
    private readonly IHttpContextAccessor _contextAccessor = httpContextAccessor;

    public string? ClientId => _contextAccessor.HttpContext?.User?.FindFirst("client_id")?.Value;
    public string? UserId => _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    public string? Email => _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
    public string? Tenant => _contextAccessor.HttpContext?.User?.FindFirst("tenant")?.Value;
    public IReadOnlyList<string> Divisions => ClaimToList(_contextAccessor.HttpContext?.User?.FindFirst("divisions"));
    
    private static List<string> ClaimToList(Claim? claim)
    {
        if (claim == null || claim.Value == null)
            return [];

        return JsonSerializer.Deserialize<List<string>>(claim.Value) ?? [];
    }
}
