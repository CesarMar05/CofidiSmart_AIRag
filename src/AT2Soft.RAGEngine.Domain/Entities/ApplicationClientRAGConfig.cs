using System;

namespace AT2Soft.RAGEngine.Domain.Entities;

public class ApplicationClientRAGConfig
{
    public Guid ApplicationClientPromptId { get; set; }
    public Guid ApplicationClientId { get; set; }
    public string Tenant { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;
    public int TargetTokens { get; set; } = 350;
    public int MaxTokens { get; set; } = 450;
    public int MinTokens { get; set; } = 150;
    public int OverlapTokens { get; set; } = 60; 
}
