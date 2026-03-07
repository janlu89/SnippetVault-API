namespace SnippetVault.Domain.Entities
{
    public class Snippet
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
        public bool IsPublic { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string CodeBody { get; set; }
        public string Language { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<SnippetTag> SnippetTags { get; set; }

    }
}
