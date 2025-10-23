using AT2Soft.RAGEngine.Application.Abstractions.DTOs;
using AT2Soft.RAGEngine.Application.Persistence.Interfaces;
using AT2Soft.RAGEngine.Domain.Entities;
using AT2Soft.RAGEngine.Infrastructure.SQLServer.Data;
using Microsoft.EntityFrameworkCore;

namespace AT2Soft.RAGEngine.Infrastructure.SQLServer.Repository;


public class RagIngestJobRepository(RAGSqlServerDbContext context) : RepositoryBase<RagIngestJob, Guid>(context), IRagIngestJobRepository
{
    public Task<bool> ExistDigest(Guid applicationId, string digest, CancellationToken cancellationToken = default)
    {
        return _context.RagIngestJobs
            .AnyAsync(rij => rij.ApplicationId == applicationId && rij.Digest == digest, cancellationToken);
    }

    public async Task<RagIngestJobInfo?> FindRagIngestJobInfoByIdAsync(Guid appId, string tenantId, Guid id, CancellationToken cancellationToken = default)
    {
        var info = await _context.RagIngestJobs
            .Select(rij => new
            {
                rij.Id,
                rij.ApplicationId,
                rij.TenantId,
                rij.SourceType,
                rij.Source,
                rij.TextLength,
                rij.TextChunkerOptions,
                rij.Metadata,
                rij.Status,
                rij.CreatedAtUtc,
                rij.CompletedAtUtc,
                rij.Error,
                rij.Digest,
                rij.ChunksTotal,
                rij.ChunksProcess
            })
            .FirstOrDefaultAsync(rij => rij.ApplicationId == appId && rij.TenantId == tenantId && rij.Id == id, cancellationToken);

        return info == null
            ? null
            : new RagIngestJobInfo(
                info.Id,
                info.ApplicationId,
                info.TenantId,
                info.SourceType,
                info.Source,
                info.TextLength,
                info.TextChunkerOptions,
                info.Metadata,
                info.Status,
                info.CreatedAtUtc,
                info.CompletedAtUtc,
                info.Error,
                info.Digest,
                info.ChunksProcess,
                info.ChunksTotal
            );
    }
    
    public async Task<IEnumerable<RagIngestJobInfo>> GetRagIngestJobInfoListAsync(Guid appId, string tenantId, CancellationToken cancellationToken = default)
    {
        var infos = await _context.RagIngestJobs
            .Where(rij => rij.ApplicationId == appId && rij.TenantId == tenantId)
            .Select(rij => new
            {
                rij.Id,
                rij.ApplicationId,
                rij.TenantId,
                rij.SourceType,
                rij.Source,
                rij.TextLength,
                rij.TextChunkerOptions,
                rij.Metadata,
                rij.Status,
                rij.CreatedAtUtc,
                rij.CompletedAtUtc,
                rij.Error,
                rij.Digest,
                rij.ChunksTotal,
                rij.ChunksProcess
            })
            .ToArrayAsync(cancellationToken);

        return infos
            .Select(info => new RagIngestJobInfo(
                info.Id,
                info.ApplicationId,
                info.TenantId,
                info.SourceType,
                info.Source,
                info.TextLength,
                info.TextChunkerOptions,
                info.Metadata,
                info.Status,
                info.CreatedAtUtc,
                info.CompletedAtUtc,
                info.Error,
                info.Digest,
                info.ChunksProcess,
                info.ChunksTotal)
            );
    }
}