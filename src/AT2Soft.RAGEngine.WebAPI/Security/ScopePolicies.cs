using Microsoft.AspNetCore.Authorization;

namespace AT2Soft.RAGEngine.WebAPI.Security;

public static class ScopePolicies
{
    public const string Ingest = "scope:ingest";
    public const string Query  = "scope:query";
    public const string Admin  = "scope:admin";

    public static void AddScopePolicies(AuthorizationOptions opt)
    {
        opt.AddPolicy(Ingest, p => p.RequireAssertion(ctx =>
            ctx.User.HasClaim(c => c.Type == "scope" && c.Value.Split(' ').Contains("ingest"))));

        opt.AddPolicy(Query, p => p.RequireAssertion(ctx =>
            ctx.User.HasClaim(c => c.Type == "scope" && c.Value.Split(' ').Contains("query"))));

        opt.AddPolicy(Admin, p => p.RequireAssertion(ctx =>
            ctx.User.HasClaim(c => c.Type == "scope" && c.Value.Split(' ').Contains("admin"))));
    }
}
