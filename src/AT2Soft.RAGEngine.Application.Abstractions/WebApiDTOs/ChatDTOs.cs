using System;

namespace AT2Soft.RAGEngine.Application.Abstractions.WebApiDTOs;

public sealed record ChatAskRequest(string Question, bool SearchContext = true);
public sealed record ChatAskResponse(string Response, bool UsedContext);
