using System.Text.Json;
using AT2Soft.RAGEngine.Application.Abstractions.KnowledgeDocument;
using AT2Soft.RAGEngine.Application.Abstractions.TextChunker;
using AT2Soft.RAGEngine.Application.Interfaces;
using AT2Soft.RAGEngine.Application.Interfaces.Repositories;
using AT2Soft.RAGEngine.Application.Interfaces.Services;
using AT2Soft.RAGEngine.Domain.Entities;
using AT2Soft.RAGEngine.Domain.Enums;
using AT2Soft.RAGEngine.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace AT2Soft.RAGEngine.Application.Features.Ingest.Services;

public class IngestTextService : IIngestTextService
{
    private readonly IRagIngestJobRepository _ragIngestJobRepository;
    private readonly IUnitOfWorkRelational _unitOfWorkRelational;
    private readonly ILogger<IngestTextService> _log;
    private readonly ITextChunkerService _textChunkerService;
    private readonly IAIModelService _ollamaService;
    private readonly IPointRepository _pointRepository;
    private readonly IKnowledgeDocumentRepository _kdRepository;

    public IngestTextService(IRagIngestJobRepository ragIngestJobRepository, ILogger<IngestTextService> log, IUnitOfWorkRelational unitOfWorkRelational, ITextChunkerService textChunkerService, IAIModelService ollamaService, IPointRepository pointRepository, IKnowledgeDocumentRepository kdRepository)
    {
        _ragIngestJobRepository = ragIngestJobRepository;
        _log = log;
        _unitOfWorkRelational = unitOfWorkRelational;
        _textChunkerService = textChunkerService;
        _ollamaService = ollamaService;
        _pointRepository = pointRepository;
        _kdRepository = kdRepository;
    }

    public async Task ProcessAsync(Guid jobId, CancellationToken cancellationToken)
    {
        // 1) Extraer Job            
        var job = await _ragIngestJobRepository.GetByIdAsync(jobId, cancellationToken);
        if (job is null) return;

        try
        {
            job.Status = IngestStatus.Processing;
            await _unitOfWorkRelational.SaveChangesAsync(cancellationToken);

            // 2) Extraer texto y datos
            var text = job.TextContent;
            var textChunkerOptions = JsonSerializer.Deserialize<TextChunkerOptions>(job.TextChunkerOptions)
                ?? throw new ApplicationException("Invalid TextChunkerOptions data");
            var metadata = JsonSerializer.Deserialize<KDMetadataRequest>(job.Metadata)
                ?? throw new ApplicationException("Invalid Metadata data");

            // 3) Chunking 
            var chunks = _textChunkerService.Chunk(text, textChunkerOptions);
            if (chunks.Count == 0)
                throw new ApplicationException("No fue posible obtner Chunks del texto");

            job.ChunksTotal = chunks.Count;
            await _unitOfWorkRelational.SaveChangesAsync(cancellationToken);

            var kdChunks = chunks
                .Select((chunk, index) => new Chunk
                {
                    Id = Guid.NewGuid(),
                    KDId = job.Id,
                    Position = chunk.Index,
                    Content = chunk.Text,
                    EstimatedTokens = chunk.EstimatedTokens
                })
                .ToList();

            // 4) Embeddings (bge-m3 en Ollama) -> float[]
            List<Point> points = [];
            foreach (var c in kdChunks)
            {
                var embedding = await _ollamaService.EmbeddingTextAsync(c.Content, cancellationToken);
                var point = new Point
                {
                    Id = c.Id.ToString(),
                    Vector = embedding,
                    Payload = new Payload
                    {
                        ApplicationId = job.ApplicationId.ToString(),
                        TenantId = job.TenantId,
                        Topic = job.Source,
                        Tags = metadata.Tags,
                        KnowledgeDocumentId = c.KDId,
                        Position = c.Position,
                        Text = c.Content
                    }
                };

                points.Add(point);

                job.ChunksProcess++;
                await _unitOfWorkRelational.SaveChangesAsync(cancellationToken);
            }

            // 5) Upsert en Qdrant con payload (ApplicationId, TenantId, docId, page, etc.)
            await _pointRepository.InsertAsync(points, cancellationToken);

            // 6) Guardar metadata en SQL (KnowledgeDocument + Chunks + pointIds)
            var kdAdd = new Domain.Entities.KnowledgeDocument
            {
                Id = job.Id,
                ApplicationClientId = job.ApplicationId,
                TenantId = job.TenantId,
                Topic = metadata.Topic,
                Tags = metadata.Tags,
                Source = job.Source,
                Title = metadata.Title,
                Description = metadata.Description,
                CollectionName = string.Empty,
                Type = job.SourceType,
                Digest = job.Digest,
                ReceivedAt = job.CreatedAtUtc,
                LastUpdate = DateTime.UtcNow,
                Chunks = kdChunks
            };

            // Insert  KnowledgeDocument
            var entity = await _kdRepository.AddAsync(kdAdd, cancellationToken);
            await _unitOfWorkRelational.SaveChangesAsync(cancellationToken);

            job.Status = IngestStatus.Succeeded;
            job.CompletedAtUtc = DateTime.UtcNow;
            await _unitOfWorkRelational.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Ingest failed {JobId}", jobId);
            job.Status = IngestStatus.Failed;
            job.Error = ex.Message;
            job.CompletedAtUtc = DateTime.UtcNow;
            await _unitOfWorkRelational.SaveChangesAsync(cancellationToken);
        }
    }
}
