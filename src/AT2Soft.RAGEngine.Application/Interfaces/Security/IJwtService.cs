using System;
using AT2Soft.RAGEngine.Application.DTOs;
using Microsoft.VisualBasic;

namespace AT2Soft.RAGEngine.Application.Interfaces.Security;

public interface IJwtService
{
    TokenResponse GenerateToken(Guid appId, string userId, string tenant, IReadOnlyList<string> divisions, string scope);
}
