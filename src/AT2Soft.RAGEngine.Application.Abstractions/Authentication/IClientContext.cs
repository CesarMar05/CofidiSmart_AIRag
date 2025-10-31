
namespace AT2Soft.RAGEngine.Application.Abstractions.Authentication;

public interface IClientContext
{
    string? ClientId { get; }
    string? UserId { get; }
    string? Email { get; }
    string? Tenant { get; }
    IReadOnlyList<string> Divisions { get; }
}