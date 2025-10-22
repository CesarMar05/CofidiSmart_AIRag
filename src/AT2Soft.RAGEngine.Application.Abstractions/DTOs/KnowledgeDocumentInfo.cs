using AT2Soft.RAGEngine.Domain.Entities;
using AT2Soft.RAGEngine.Domain.Enums;

namespace AT2Soft.RAGEngine.Application.Abstractions.DTOs;

public sealed record KnowledgeDocumentInfo
(
    Guid Id,
    Guid ApplicationClientId,
    string TenantId,
    string Title,
    KnowledgeDocumentType Type,
    string Source,
    string Description,
    string Topic,
    string Digest,
    List<string> Tags,
    string CollectionName,
    DateTime ReceivedAt,
    DateTime LastUpdate,
    List<ChunkInfo>? Chunks
)
{
    // Conversión implícita de DTO → Entidad
    public static implicit operator Domain.Entities.KnowledgeDocument(KnowledgeDocumentInfo dto)
        => new()
        {
            Id = dto.Id,
            Digest = dto.Digest,
            ApplicationClientId = dto.ApplicationClientId,
            TenantId = dto.TenantId,
            Topic = dto.Topic,
            Tags = dto.Tags,
            Description = dto.Description,
            Title = dto.Title,
            Type = dto.Type,
            Source = dto.Source,
            CollectionName = dto.CollectionName,
            ReceivedAt = dto.ReceivedAt,
            LastUpdate = dto.LastUpdate,
            ApplicationClient = new ApplicationClient(),
            Chunks = []
        };

    // Conversión implícita de Entidad → DTO
    public static implicit operator KnowledgeDocumentInfo(Domain.Entities.KnowledgeDocument entity) =>
        new(
            entity.Id,
            entity.ApplicationClientId,
            entity.TenantId,
            entity.Title,
            entity.Type,
            entity.Source,
            entity.Description,
            entity.Topic,
            entity.Digest,
            entity.Tags,
            entity.CollectionName,
            entity.ReceivedAt,
            entity.LastUpdate,
            entity.Chunks?.Select(c => new ChunkInfo(c.Id, c.Position, c.Content)).OrderBy(c => c.Position).ToList()
        );
}
