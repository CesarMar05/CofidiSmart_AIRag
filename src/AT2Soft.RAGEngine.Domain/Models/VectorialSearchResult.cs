using System;
using AT2Soft.RAGEngine.Domain.Entities;

namespace AT2Soft.RAGEngine.Domain.Models;

public sealed record VectorialSearchResult
(
    Guid Id,
    int Version,
    double Score,
    Payload? Payload
);
