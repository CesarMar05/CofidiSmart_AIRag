using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AT2Soft.RAGEngine.Application.DTOs;
using AT2Soft.RAGEngine.Application.Interfaces.Security;
using Microsoft.IdentityModel.Tokens;

namespace AT2Soft.RAGEngine.WebAPI.Security;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public TokenResponse GenerateToken(Guid appId, string scope)
    {
        // ====== Cargar configuración ======
        var jwtCfg = _configuration.GetSection("Jwt");
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtCfg["SigningKey"]!));
        var issuer = jwtCfg["Issuer"];
        var audience = jwtCfg["Audience"];
        var tokenMinutes = int.Parse(jwtCfg["TokenLifetimeMinutes"] ?? "60");

        var claims = new List<Claim>
        {
            new("client_id", appId.ToString()),
            new("scope", scope)
        };

        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(tokenMinutes),
            signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return new TokenResponse(jwt, "Bearer", tokenMinutes * 60, scope);
    }
}
