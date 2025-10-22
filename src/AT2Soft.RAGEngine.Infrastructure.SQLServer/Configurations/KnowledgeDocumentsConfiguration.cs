using AT2Soft.RAGEngine.Domain.Entities;
using AT2Soft.RAGEngine.Infrastructure.SQLServer.Converts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AT2Soft.RAGEngine.Infrastructure.SQLServer.Configurations;

public class KnowledgeDocumentConfiguration : IEntityTypeConfiguration<KnowledgeDocument>
{
    public void Configure(EntityTypeBuilder<KnowledgeDocument> builder)
    {
        builder.ToTable("KnowledgeDocuments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.TenantId)
           .IsRequired()
           .HasMaxLength(20);

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(a => a.Type)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(a => a.Source)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(a => a.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.Topic)
            .IsRequired()
            .HasMaxLength(12);

        builder.Property(a => a.Digest)
            .IsRequired()
            .HasMaxLength(24);

        builder.Property(a => a.Tags)
            .HasJsonListConversion<string>();

        builder.Property(a => a.ReceivedAt)
            .IsRequired();

        builder.Property(a => a.LastUpdate)
            .IsRequired();

        builder
            .HasMany(k => k.Chunks)
            .WithOne(c => c.KnowledgeDocument!)
            .HasForeignKey(c => c.KDId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(kd => new { kd.ApplicationClientId, kd.Digest })
               .IsUnique();
    }
}
