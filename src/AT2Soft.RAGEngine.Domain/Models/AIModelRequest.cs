using System;

namespace AT2Soft.RAGEngine.Domain.Models;


public class AIModelRequest
{
    public string Model { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;
    public bool Stream { get; set; } = false;
}