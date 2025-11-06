using System;
using AT2Soft.RAGEngine.Domain.Enums;

namespace AT2Soft.RAGEngine.Application.Abstractions.KnowledgeDocument;

public sealed record KDMetadataRequest(
    string TenantId,
    string Title,
    string Topic,
    IReadOnlyList<string> Tags,
    string Description,
    IReadOnlyList<string>? Divisions
);