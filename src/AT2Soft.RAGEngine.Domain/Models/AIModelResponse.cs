using System;

namespace AT2Soft.RAGEngine.Domain.Models;

public class AIModelResponse
{
    public string Response { get; set; } = string.Empty;
    public bool Done { get; set; }
}
