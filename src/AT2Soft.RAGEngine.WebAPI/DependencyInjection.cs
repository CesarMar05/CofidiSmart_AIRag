using System.Text;
using AT2Soft.RAGEngine.Application.Abstractions.Authentication;
using AT2Soft.RAGEngine.Application.Interfaces.Security;
using AT2Soft.RAGEngine.WebAPI.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace AT2Soft.RAGEngine.WebAPI;

public static class DependencyInjection
{
    public static IServiceCollection AddRAGJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // ====== Cargar configuración ======
        var jwtCfg = configuration.GetSection("Jwt");
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtCfg["SigningKey"]!));
        var issuer = jwtCfg["Issuer"];
        var audience = jwtCfg["Audience"];
        var tokenMinutes = int.Parse(jwtCfg["TokenLifetimeMinutes"] ?? "60");


        // ====== AuthN/Z ======
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

        services.AddAuthorization(options =>
        {
            ScopePolicies.AddScopePolicies(options);
        });

        services.AddSingleton<ISecretHasherService, SecretHasherService>();

        services.AddSingleton<IJwtService, JwtService>();

        services.AddHttpContextAccessor();
        services.AddScoped<IClientContext, ClientContext>();
        
        return services;
    }
}
