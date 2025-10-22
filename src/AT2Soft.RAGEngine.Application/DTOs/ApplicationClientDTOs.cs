using System;
using AT2Soft.RAGEngine.Domain.Entities;

namespace AT2Soft.RAGEngine.Application.DTOs;

public sealed record AppClientAddResult(ApplicationClient ApplicationClient, string SecretKey);