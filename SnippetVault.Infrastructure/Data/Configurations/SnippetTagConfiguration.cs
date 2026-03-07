using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SnippetVault.Domain.Entities;

namespace SnippetVault.Infrastructure.Data.Configurations
{
    public class SnippetTagConfiguration : IEntityTypeConfiguration<SnippetTag>
    {
        public void Configure(EntityTypeBuilder<SnippetTag> builder)
        {
            builder.HasKey(st => new { st.SnippetId, st.TagId });
        }
    }
}