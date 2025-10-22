using System.Security.Cryptography;
using AT2Soft.RAGEngine.Application.Interfaces.Security;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace AT2Soft.RAGEngine.WebAPI.Security;

public class SecretHasherService : ISecretHasherService
{
    public string Hash(string secret, byte[]? salt = null)
    {
        salt ??= RandomNumberGenerator.GetBytes(16);
        var hash = KeyDerivation.Pbkdf2(
            password: secret,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100_000,
            numBytesRequested: 32);
        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }


    public bool Verify(string appKey, string clientSecretHash)
    {
        var parts = clientSecretHash.Split('.');
        if (parts.Length != 2) return false;
        var salt = Convert.FromBase64String(parts[0]);
        var expected = parts[1];
        var candidate = Hash(appKey, salt).Split('.')[1];
        return candidate == expected;
    }
}
