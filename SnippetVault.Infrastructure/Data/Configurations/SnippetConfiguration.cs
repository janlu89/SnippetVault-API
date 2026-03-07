using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SnippetVault.Domain.Entities;

namespace SnippetVault.Infrastructure.Data.Configurations
{
    public class SnippetConfiguration : IEntityTypeConfiguration<Snippet>
    {
        public void Configure(EntityTypeBuilder<Snippet> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Title)
                .IsRequired()
                .HasMaxLength(200);
            builder.Property(s => s.Description)
                .HasMaxLength(1000);
            builder.Property(s => s.CodeBody)
                .IsRequired();
            builder.Property(s => s.Language)
                .IsRequired()
                .HasMaxLength(50);
            builder.HasOne(s => s.User)
                .WithMany(u => u.Snippets)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.SnippetTags)
                .WithOne(st => st.Snippet)
                .HasForeignKey(st => st.SnippetId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(s => s.DeletedAt == null);
        }
    }
}
