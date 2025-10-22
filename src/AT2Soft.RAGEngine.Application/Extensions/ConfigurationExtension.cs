using Microsoft.Extensions.Configuration;

namespace AT2Soft.RAGEngine.Application.Extensions;

public static class ConfigurationExtension
{
    public static string MustHaveConnectionString(this IConfiguration config, string name)
    {
        var cs = config.GetConnectionString(name);
        if (string.IsNullOrWhiteSpace(cs))
            throw new InvalidOperationException($"Connection string '{name}' is required but not found.");
        
        return cs;
    }
}
