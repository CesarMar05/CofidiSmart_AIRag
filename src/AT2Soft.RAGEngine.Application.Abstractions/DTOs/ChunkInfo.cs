using System;

namespace AT2Soft.RAGEngine.Application.Abstractions.DTOs;

public sealed record ChunkInfo
(
    Guid Id,
    int Position,
    string? Content
);