using AT2Soft.RAGEngine.Domain.Entities;
using AT2Soft.RAGEngine.Infrastructure.SQLServer.Converts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AT2Soft.RAGEngine.Infrastructure.SQLServer.Configurations;

public class RagIngestJobConfiguration : IEntityTypeConfiguration<RagIngestJob>
{
    public void Configure(EntityTypeBuilder<RagIngestJob> builder)
    {
        builder.ToTable("RagIngestJobs");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.ApplicationId)
            .IsRequired();

        builder.Property(a => a.TenantId)
            .IsRequired();

        builder.Property(a => a.SourceType)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(a => a.Source)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.TextContent)
            .HasConversion(new CompressedStringConverter())
            .HasColumnType("varbinary(max)")
            .IsRequired();

        builder.Property(a => a.Metadata)
            .IsRequired();

        builder.Property(a => a.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(a => a.CreatedAtUtc)
            .IsRequired();

        builder.Property(a => a.Error)
            .HasMaxLength(200);

        builder.Property(a => a.Digest)
            .IsRequired()
            .HasMaxLength(24);

        builder.HasIndex(a => new { a.ApplicationId, a.Digest })
            .IsUnique();
    }
}