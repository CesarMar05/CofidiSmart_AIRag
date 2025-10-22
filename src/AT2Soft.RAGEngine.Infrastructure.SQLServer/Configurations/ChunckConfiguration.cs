using AT2Soft.RAGEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AT2Soft.RAGEngine.Infrastructure.SQLServer.Configurations;

public class ChunkConfiguration : IEntityTypeConfiguration<Chunk>
{
    public void Configure(EntityTypeBuilder<Chunk> builder)
    {
        builder.ToTable("Chunks");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.KDId)
            .IsRequired();

        builder.Property(a => a.Position)
            .IsRequired();

        builder.Property(a => a.Content)
            .IsRequired();

        builder.HasIndex(c => new { c.KDId, c.Position })
            .IsUnique();
    }
}
