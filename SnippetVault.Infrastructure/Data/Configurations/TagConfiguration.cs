using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SnippetVault.Domain.Entities;

namespace SnippetVault.Infrastructure.Data.Configurations
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.HasKey(t => t.Id);
            builder.HasIndex(t => t.Name)
                .IsUnique();
            builder.Property(t => t.Name).IsRequired().HasMaxLength(100);
            builder.HasMany(t => t.SnippetTags)
                   .WithOne(st => st.Tag)
                   .HasForeignKey(st => st.TagId);

            builder.HasQueryFilter(t => t.DeletedAt == null);
        }
    }
}
