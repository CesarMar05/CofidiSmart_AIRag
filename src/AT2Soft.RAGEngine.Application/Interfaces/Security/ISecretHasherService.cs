using System;

namespace AT2Soft.RAGEngine.Application.Interfaces.Security;

public interface ISecretHasherService
{
    string Hash(string secret, byte[]? salt = null);
    bool Verify(string appKey, string clientSecretHash);
}
