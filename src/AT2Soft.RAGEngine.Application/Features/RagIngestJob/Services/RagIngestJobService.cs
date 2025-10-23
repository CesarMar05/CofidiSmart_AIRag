using System.Text.Json;
using AT2Soft.Application.Result;
using AT2Soft.RAGEngine.Application.Abstractions.DTOs;
using AT2Soft.RAGEngine.Application.Abstractions.KnowledgeDocument;
using AT2Soft.RAGEngine.Application.Abstractions.Queue;
using AT2Soft.RAGEngine.Application.Abstractions.TextChunker;
using AT2Soft.RAGEngine.Application.Interfaces;
using AT2Soft.RAGEngine.Application.Interfaces.Services;
using AT2Soft.RAGEngine.Application.Persistence.Interfaces;
using AT2Soft.RAGEngine.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static AT2Soft.RAGEngine.Application.Helpers.HashHelper;

namespace AT2Soft.RAGEngine.Application.Features.RagIngestJob.Services;

public class RagIngestJobServices : IRagIngestJobServices
{
    private const HashAlgorithmType HashType = HashAlgorithmType.MD5;
    private readonly IRagIngestJobRepository _ragIngestJobRepository;
    private readonly IUnitOfWorkRelational _unitOfWorkRelational;
    private readonly IBackgroundTaskQueue _queue;


    public RagIngestJobServices(IRagIngestJobRepository ragIngestJobRepository, IUnitOfWorkRelational unitOfWorkRelational, IBackgroundTaskQueue queue)
    {
        _ragIngestJobRepository = ragIngestJobRepository;
        _unitOfWorkRelational = unitOfWorkRelational;
        _queue = queue;
    }

    public async Task<Result<Guid>> AddRagIngestJob(Guid applicationId, KDMetadataRequest metadata, TextChunkerOptions textChunkerOptions, KnowledgeDocumentType sourceType, string source, string text, CancellationToken cancellationToken = default)
    {
        var digest = GetHashBase64(text, HashType);
        var exist = await _ragIngestJobRepository.ExistDigest(applicationId, digest, cancellationToken);
        if (exist)
            return Result.Failure<Guid>(new("JobExist", $"Ya existe un Job con este Texto"));

        var newJob = new Domain.Entities.RagIngestJob
        {
            Id = Guid.NewGuid(),
            ApplicationId = applicationId,
            TenantId = metadata.TenantId,
            Metadata = JsonSerializer.Serialize(metadata),
            SourceType = sourceType,
            Source = source,
            TextContent = text,
            TextLength = text.Length,
            TextChunkerOptions = JsonSerializer.Serialize(textChunkerOptions),
            Status = IngestStatus.Pending,
            CreatedAtUtc = DateTime.UtcNow,
            CompletedAtUtc = null,
            Error = string.Empty,
            Digest = digest,
            ChunksTotal = 0,
            ChunksProcess = 0
        };

        var entity = await _ragIngestJobRepository.AddAsync(newJob, cancellationToken);
        await _unitOfWorkRelational.SaveChangesAsync(cancellationToken);

        // Encolar trabajo
        await _queue.EnqueueAsync(async (sp, token) =>
        {
            // Resuelve servicios scoped aquí
            var logger = sp.GetRequiredService<ILogger<RagIngestJobServices>>();
            var myService = sp.GetRequiredService<IIngestTextService>();

            logger.LogInformation("Ingest iniciado para {Id}", entity.Id);
            await myService.ProcessAsync(entity.Id, token);
            logger.LogInformation("Ingest finalizado para {Id}", entity.Id);
        }, cancellationToken);

        return entity.Id;
    }

    public async Task<Result<RagIngestJobInfo>> GetRagIngestJobInfo(Guid appId,  string tenantId, Guid jobId, CancellationToken cancellationToken)
    {
        var found = await _ragIngestJobRepository.FindRagIngestJobInfoByIdAsync(appId, tenantId, jobId, cancellationToken);
        return found
            ?? Result.Failure<RagIngestJobInfo>(new("RagIngestJobInfoNotFound", $"No se localizo el Job con id {jobId}"));
    }

    public async Task<Result<List<RagIngestJobInfo>>> GetRagIngestJobInfoList(Guid appId, string tenantId, CancellationToken cancellationToken)
    {
        var list = await _ragIngestJobRepository.GetRagIngestJobInfoListAsync(appId, tenantId, cancellationToken);
        return list.ToList();
    }
}
