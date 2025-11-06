using System;

namespace AT2Soft.RAGEngine.Application.Abstractions.Features.AppClient.DTOs;

public sealed record RAGConfigDto(string Prompt, int TargetTokens, int MaxTokens, int MinTokens, int OverlapTokens);
