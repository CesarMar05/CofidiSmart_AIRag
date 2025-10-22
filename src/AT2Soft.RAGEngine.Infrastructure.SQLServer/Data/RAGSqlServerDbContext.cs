using AT2Soft.RAGEngine.Domain.Entities;
using AT2Soft.RAGEngine.Infrastructure.SQLServer.Configurations;
using Microsoft.EntityFrameworkCore;

namespace AT2Soft.RAGEngine.Infrastructure.SQLServer.Data;

public class RAGSqlServerDbContext : DbContext
{
    
    public RAGSqlServerDbContext(DbContextOptions<RAGSqlServerDbContext> options) : base(options) { }

    public DbSet<ApplicationClient> ApplicationClients => Set<ApplicationClient>();
    public DbSet<KnowledgeDocument> KnowledgeDocuments => Set<KnowledgeDocument>();
    public DbSet<Chunk> Chunks => Set<Chunk>();
    public DbSet<RagIngestJob> RagIngestJobs => Set<RagIngestJob>();




    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ApplicationClientConfiguration());
        modelBuilder.ApplyConfiguration(new KnowledgeDocumentConfiguration());
        modelBuilder.ApplyConfiguration(new ChunkConfiguration());
        modelBuilder.ApplyConfiguration(new RagIngestJobConfiguration());
    }
}