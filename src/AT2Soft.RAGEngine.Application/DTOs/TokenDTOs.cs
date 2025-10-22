using System;

namespace AT2Soft.RAGEngine.Application.DTOs;

public sealed record TokenRequest(string AppId, string AppKey, string? Scope);
public sealed record TokenResponse(string AccessToken, string TokenType, int ExpiresIn, string Scope);
