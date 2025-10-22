using AT2Soft.RAGEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AT2Soft.RAGEngine.Infrastructure.SQLServer.Configurations;

public class KnowledgeConfiguration : IEntityTypeConfiguration<Knowledge>
{
    public void Configure(EntityTypeBuilder<Knowledge> builder)
    {
        builder.ToTable("Knowledges");

        builder.HasKey(a => a.KnowledgeId);

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(12);

        builder.Property(a => a.Description)
            .IsRequired()
            .HasMaxLength(150); 
    }
}