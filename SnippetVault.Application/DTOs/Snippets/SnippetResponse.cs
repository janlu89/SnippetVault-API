namespace SnippetVault.Application.DTOs.Snippets
{
    public class SnippetResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string CodeBody { get; set; }
        public string Language { get; set; }
        public bool IsPublic { get; set; }
        public string Username { get; set; }
        public List<string> Tags { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
