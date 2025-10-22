using System;

namespace AT2Soft.RAGEngine.WebAPI.Abstractions.Models;

public class SearchRequest
{
    public string Keyword { get; set; } = string.Empty;
    public string Query { get; set; } = string.Empty;
    public int TopK { get; set; } = 3;

}
