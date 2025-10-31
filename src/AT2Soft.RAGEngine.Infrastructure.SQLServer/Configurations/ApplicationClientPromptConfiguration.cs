using System;
using AT2Soft.RAGEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AT2Soft.RAGEngine.Infrastructure.SQLServer.Configurations;

public class ApplicationClientPromptConfiguration : IEntityTypeConfiguration<ApplicationClientPrompt>
{
    public void Configure(EntityTypeBuilder<ApplicationClientPrompt> builder)
    {
        builder.ToTable("ApplicationClientPrompts");

        builder.HasKey(a => a.ApplicationClientPromptId);

        builder.Property(a => a.ApplicationClientId)
            .IsRequired();

        builder.Property(a => a.Tenant)
           .IsRequired()
           .HasMaxLength(20);

        builder.Property(a => a.Prompt)
            .IsRequired();

    }
}
