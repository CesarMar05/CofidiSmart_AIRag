using AT2Soft.RAGEngine.Domain.Entities;

namespace AT2Soft.RAGEngine.Application.Abstractions.DTOs;

public sealed record ApplicationClientInfo
(
    Guid ApplicationClientId,
    string Name,
    string ClientSecretHash,
    string AllowedScopes,
    bool IsActive,
    List<KnowledgeDocumentInfo>? KnowledgeDocuments
)
{
    // Conversión implícita de DTO → Entidad
    public static implicit operator ApplicationClient(ApplicationClientInfo dto)
        => new()
        {
            AllowedScopes = dto.AllowedScopes,
            ClientSecretHash =  dto.ClientSecretHash,
            IsActive = dto.IsActive,
            Name = dto.Name,
            KnowledgeDocuments = []
        };

    // Conversión implícita de Entidad → DTO
    public static implicit operator ApplicationClientInfo(ApplicationClient entity) =>
        new(
            entity.ApplicationClientId,
            entity.Name,
            entity.ClientSecretHash,
            entity.AllowedScopes,
            entity.IsActive,
            entity.KnowledgeDocuments?.Select(kd => (KnowledgeDocumentInfo)kd).OrderBy(kd => kd.LastUpdate).ToList()
        );
}
