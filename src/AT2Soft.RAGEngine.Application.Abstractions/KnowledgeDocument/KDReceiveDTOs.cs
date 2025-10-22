using System;
using AT2Soft.RAGEngine.Application.Abstractions.TextChunker;

namespace AT2Soft.RAGEngine.Application.Abstractions.KnowledgeDocument;

public sealed record KDFileRequest(string FileName, byte[] FileContent);

public sealed record KDReceiveTextRequest(KDMetadataRequest Metadata, string Text, TextChunkerOptions? TextChunkerOptions = null);

public sealed record KDReceiveFileRequest(KDMetadataRequest Metadata, KDFileRequest File, TextChunkerOptions? TextChunkerOptions = null);

public sealed record KDReceiveUrlRequest(KDMetadataRequest Metadata, string Url, TextChunkerOptions? TextChunkerOptions = null);