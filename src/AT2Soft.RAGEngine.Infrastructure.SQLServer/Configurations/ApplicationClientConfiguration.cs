using AT2Soft.RAGEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AT2Soft.RAGEngine.Infrastructure.SQLServer.Configurations;

public class ApplicationClientConfiguration : IEntityTypeConfiguration<ApplicationClient>
{
    public void Configure(EntityTypeBuilder<ApplicationClient> builder)
    {
        builder.ToTable("ApplicationClients");

        builder.HasKey(a => a.ApplicationClientId);

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(12);

        builder.Property(a => a.ClientSecretHash)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(a => a.Name)
            .IsUnique();

        builder.HasMany(ac => ac.KnowledgeDocuments)
            .WithOne(kd => kd.ApplicationClient)
            .HasForeignKey(kd => kd.ApplicationClientId)
            .IsRequired(); 
    }
}
