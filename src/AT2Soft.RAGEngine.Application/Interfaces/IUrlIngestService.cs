using System;

namespace AT2Soft.RAGEngine.Application.Interfaces;

public interface IHttpService
{
    Task<string> GetUrlContentAsync(string url);
}
