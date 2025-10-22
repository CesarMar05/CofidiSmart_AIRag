using System;

namespace AT2Soft.RAGEngine.Application.Abstractions.TextExtractor;

public sealed record TextExtractorUrlRequest(string Url);

public sealed record TextExtractorResponse(string Source, int Length, TimeSpan TimeSpan, string Text);