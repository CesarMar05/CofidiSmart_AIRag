using System.Text.Json;
using AT2Soft.Application.Result;
using AT2Soft.RAGEngine.Application.Abstractions.Authentication;
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
    private readonly IClientContext _clientContext;


    public RagIngestJobServices(IRagIngestJobRepository ragIngestJobRepository, IUnitOfWorkRelational unitOfWorkRelational, IBackgroundTaskQueue queue, IClientContext clientContext)
    {
        _ragIngestJobRepository = ragIngestJobRepository;
        _unitOfWorkRelational = unitOfWorkRelational;
        _queue = queue;
        _clientContext = clientContext;
    }

    public async Task<Result<Guid>> AddRagIngestJob(KDMetadataRequest metadata, TextChunkerOptions textChunkerOptions, KnowledgeDocumentType sourceType, string source, string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_clientContext.ClientId) || string.IsNullOrWhiteSpace(_clientContext.Tenant))
            return Result.Failure<Guid>(new("Unauthorized", $"Token no válido"));

        if (_clientContext.Tenant != metadata.TenantId)
            return Result.Failure<Guid>(new("Unauthorized", $"Token no tiene autorización para el tenant {metadata.TenantId} válido"));

        if (_clientContext.Divisions.Count > 0)
        {
            if (metadata.Divisions == null || metadata.Divisions.Count == 0)
                return Result.Failure<Guid>(new("Unauthorized", $"Token no tiene autorización para hacer ingest de documentos sin division"));
            if (!metadata.Divisions.All(d => _clientContext.Divisions.Contains(d)))
                return Result.Failure<Guid>(new("Unauthorized", $"Token no tiene autorización para hacer ingest de las divisiones {string.Join(" ",metadata.Divisions)}"));
        }

        Guid applicationId = new(_clientContext.ClientId);

        var digest = GetHashBase64(text, HashType);
        var exist = await _ragIngestJobRepository.ExistDigest(applicationId, digest, cancellationToken);
        if (exist)
            return Result.Failure<Guid>(new("JobExist", $"Ya existe un Job con este Texto"));

        var newJob = new Domain.Entities.RagIngestJob
        {
            Id = Guid.NewGuid(),
            ApplicationId = applicationId,
            TenantId = _clientContext.Tenant,
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

    public async Task<Result<RagIngestJobInfo>> GetRagIngestJobInfo(Guid jobId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_clientContext.ClientId) || string.IsNullOrWhiteSpace(_clientContext.Tenant))
            return Result.Failure<RagIngestJobInfo>(new("Unauthorized", $"Token no válido"));
            
        var found = await _ragIngestJobRepository.FindRagIngestJobInfoByIdAsync(new Guid(_clientContext.ClientId), _clientContext.Tenant, jobId, cancellationToken);
        return found
            ?? Result.Failure<RagIngestJobInfo>(new("RagIngestJobInfoNotFound", $"No se localizo el Job con id {jobId}"));
    }

    public async Task<Result<List<RagIngestJobInfo>>> GetRagIngestJobInfoList(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_clientContext.ClientId) || string.IsNullOrWhiteSpace(_clientContext.Tenant))
            return Result.Failure<List<RagIngestJobInfo>>(new("Unauthorized", $"Token no válido"));

        var list = await _ragIngestJobRepository.GetRagIngestJobInfoListAsync(new Guid(_clientContext.ClientId), _clientContext.Tenant, cancellationToken);
        return list.ToList();
    }
}
