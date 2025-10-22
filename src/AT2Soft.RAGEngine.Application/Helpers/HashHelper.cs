using System.Security.Cryptography;
using System.Text;

namespace AT2Soft.RAGEngine.Application.Helpers;

public class HashHelper
{
    public enum HashAlgorithmType
    {
        MD5,
        SHA1,
        SHA256,
        SHA384,
        SHA512
    }
    public static string GetHashBase64(string input, HashAlgorithmType hashType)
    {
        using HashAlgorithm? algorithm = hashType switch
        {
            HashAlgorithmType.MD5 => MD5.Create(),
            HashAlgorithmType.SHA1 => SHA1.Create(),
            HashAlgorithmType.SHA256 => SHA256.Create(),
            HashAlgorithmType.SHA384 => SHA384.Create(),
            HashAlgorithmType.SHA512 => SHA512.Create(),
            _ => throw new ArgumentException($"Algoritmo no soportado: {hashType}")
        };

        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] hashBytes = algorithm.ComputeHash(inputBytes);
        return Convert.ToBase64String(hashBytes);
    }
}
