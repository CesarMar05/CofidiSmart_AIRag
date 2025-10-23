using AT2Soft.Application.Result;
using AT2Soft.RAGEngine.Application.Abstractions.DTOs;
using AT2Soft.RAGEngine.Application.Abstractions.TextChunker;
using AT2Soft.RAGEngine.Application.Interfaces;
using AT2Soft.RAGEngine.Application.Persistence.Interfaces;
using AT2Soft.RAGEngine.Domain.Entities;
using AT2Soft.RAGEngine.Domain.Enums;
using AT2Soft.RAGEngine.Domain.Interfaces.Services;
using static AT2Soft.RAGEngine.Application.Helpers.HashHelper;

namespace AT2Soft.RAGEngine.Application.Features.KnowledgeDocument.Services;

public class KnowledgeDocumentServices : IKnowledgeDocumentServices
{
    private const HashAlgorithmType HashType = HashAlgorithmType.MD5;
    private readonly IAIModelService _ollamaService;
    private readonly IPointRepository _pointRepository;
    private readonly IKnowledgeDocumentRepository _kdRepository;
    private readonly IUnitOfWorkRelational _unitOfWork;
    private readonly IChunkRepository _chunkRepository;
    private readonly ITextChunkerService _textChunkerService;


    public KnowledgeDocumentServices(IAIModelService ollamaService, IPointRepository pointRepository, IKnowledgeDocumentRepository kdRepository, IUnitOfWorkRelational unitOfWork, IChunkRepository chunkRepository, ITextChunkerService textChunkerService)
    {
        _ollamaService = ollamaService;
        _pointRepository = pointRepository;
        _kdRepository = kdRepository;
        _unitOfWork = unitOfWork;
        _chunkRepository = chunkRepository;
        _textChunkerService = textChunkerService;
    }

    public async Task<Result<KnowledgeDocumentInfo>> InsertPlainTextAsyn(Guid appId, string tenantId, string topic, List<string> tags, KnowledgeDocumentType sourceType, string sourceId, string title, string description, string text, TextChunkerOptions textChunkerOptions, CancellationToken cancellationToken)
    {
        try
        {
            var digest = GetHashBase64(text, HashType);

            var existKD = await _kdRepository.ExistDigest(appId, digest, cancellationToken);
            if (existKD)
                return Result.Failure<KnowledgeDocumentInfo>(new("KD_ExistDigest", "Ya existe este Texto como KnowledgeDocument"));

            // Get Chunks
            //var chunks = TextCharacterSpliter(text, textChunkerOptions.TargetTokens, textChunkerOptions.OverlapTokens);
            var chunks = _textChunkerService.Chunk(text, textChunkerOptions);

            // Get Embeddings (Vetores)
            var embeddings = await _ollamaService.EmbeddingTextListAsync(chunks.Select(c => c.Text).ToList(), cancellationToken);

            // Set KnowledgeDocument
            var kdAdd = new Domain.Entities.KnowledgeDocument
            {
                Id = Guid.NewGuid(),
                ApplicationClientId = appId,
                TenantId = tenantId,
                Topic = topic,
                Tags = tags,
                Source = sourceId,
                Title = title,
                Description = description,
                CollectionName = string.Empty,
                Type = sourceType,
                Digest = digest,
                ReceivedAt = DateTime.UtcNow,
                LastUpdate = DateTime.UtcNow,
                Chunks = chunks
                    .Select((chunk, index) => new Chunk
                    {
                        Id = Guid.NewGuid(),
                        KDId = Guid.Empty,
                        Position = chunk.Index,
                        Content = chunk.Text,
                        EstimatedTokens = chunk.EstimatedTokens
                    })
                    .ToList()
            };

            // Insert  KnowledgeDocument
            var entity = await _kdRepository.AddAsync(kdAdd, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Insert Poins
            List<(Chunk chunk, float[] vector)> data = kdAdd.Chunks.Zip(embeddings).ToList();
            List<Point> points = data.Select(c => new Point
            {
                Id = c.chunk.Id.ToString(),
                Vector = c.vector,
                Payload = new Payload
                {
                    ApplicationId = appId.ToString(),
                    TenantId = tenantId,
                    Topic = kdAdd.Source,
                    Tags = tags,
                    KnowledgeDocumentId = c.chunk.KDId,
                    Position = c.chunk.Position,
                    Text = c.chunk.Content,

                }
            }).ToList();
            await _pointRepository.InsertAsync(points, cancellationToken);

            return (KnowledgeDocumentInfo)entity;
        }
        catch (Exception ex)
        {
            return Result.Failure<KnowledgeDocumentInfo>(new Error("KD_InserTextException", $"Falló la inserción de Texto como KnowledgeDocument. {ex.Message}"));
        }
    }

    public async Task<string> GetChunkContext(Guid chunkPointId, string chunkContent, CancellationToken cancellationToken = default)
    {
        var chuck = await _chunkRepository.GetByPointIdAsync(chunkPointId, cancellationToken);
        if (chuck == null)
            return chunkContent;

        var context = string.Empty;
        if (chuck.Position > 1)
            context = chuck.Content[..^50];

        context += chunkContent;

        if (chuck.KnowledgeDocument != null && chuck.KnowledgeDocument.Chunks != null)
        {
            var chucklast = chuck.KnowledgeDocument.Chunks
                .FirstOrDefault(c => c.Position == chuck.Position + 1);
            context += chucklast != null
                ? chucklast.Content[50..]
                : string.Empty;
        }

        return context;
    }

    public async Task<Result<List<KnowledgeDocumentInfo>>> GetList(Guid applicationId, string tenant, int take, CancellationToken cancellationToken = default)
    {
        int? ttake = take == 0 ? null : take;

        var listRslt = await _kdRepository.GetListAsync(kd => kd.ApplicationClientId == applicationId && kd.TenantId == tenant, take: ttake);
        return listRslt
            .Select(kd => (KnowledgeDocumentInfo)kd)
            .OrderByDescending(kd => kd.LastUpdate)
            .ToList();
    }

    public async Task<Result<KnowledgeDocumentInfo?>> GetById(Guid applicationId, string tenant, Guid id, bool fullData, CancellationToken cancellationToken = default)
    {
        var kd = fullData
            ? await _kdRepository.GetFullDataByIdAsync(id, cancellationToken)
            : await _kdRepository.GetByIdAsync(id, cancellationToken);

        return kd != null && kd.ApplicationClientId == applicationId && kd.TenantId == tenant
            ? (KnowledgeDocumentInfo)kd
            : null;
    }
}
