using System;

namespace AT2Soft.RAGEngine.Domain.Entities;

public class ApplicationClientPrompt
{
    public Guid ApplicationClientPromptId { get; set; }
    public Guid ApplicationClientId { get; set; }
    public string Tenant { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;
}
