using System;
using AT2Soft.RAGEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AT2Soft.RAGEngine.Infrastructure.SQLServer.Configurations;

public class ApplicationClientRAGConfigConfiguration : IEntityTypeConfiguration<ApplicationClientRAGConfig>
{
    public void Configure(EntityTypeBuilder<ApplicationClientRAGConfig> builder)
    {
        builder.ToTable("ApplicationClientRAGConfigs");

        builder.HasKey(a => a.ApplicationClientPromptId);

        builder.Property(a => a.ApplicationClientId)
            .IsRequired();

        builder.Property(a => a.Tenant)
           .IsRequired()
           .HasMaxLength(20);

        builder.Property(a => a.Prompt)
            .IsRequired();

        builder.Property(a => a.TargetTokens)
            .IsRequired();

        builder.Property(a => a.MaxTokens)
            .IsRequired();

        builder.Property(a => a.MinTokens)
            .IsRequired();

        builder.Property(a => a.OverlapTokens)
            .IsRequired();


        builder.HasIndex(e => new { e.ApplicationClientId, e.Tenant })
            .IsUnique();
    }
}
