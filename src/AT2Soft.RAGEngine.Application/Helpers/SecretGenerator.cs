using System.Security.Cryptography;

namespace AT2Soft.RAGEngine.Application.Helpers;

public class SecretGenerator
{
    // Devuelve un secreto URL-safe, ~43 chars con 32 bytes (256 bits) de entropía
    public static string Generate(int bytes = 32)
    {
        var raw = RandomNumberGenerator.GetBytes(bytes);
        var b64 = Convert.ToBase64String(raw)
            .TrimEnd('=').Replace('+', '-').Replace('/', '_');
        return b64;
    }
}
