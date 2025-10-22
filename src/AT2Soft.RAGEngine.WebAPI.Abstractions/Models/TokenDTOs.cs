using System;

namespace AT2Soft.RAGEngine.WebAPI.Abstractions.Models;

public record IngestDto(string Title, string Text, string? Collection);
public record QueryDto(string Question, string? Collection);
