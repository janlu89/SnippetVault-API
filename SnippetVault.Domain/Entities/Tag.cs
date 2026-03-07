namespace SnippetVault.Domain.Entities
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<SnippetTag> SnippetTags { get; set; }
    }
}

