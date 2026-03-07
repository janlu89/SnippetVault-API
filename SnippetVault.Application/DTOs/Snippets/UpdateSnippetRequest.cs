namespace SnippetVault.Application.DTOs.Snippets
{
    public class UpdateSnippetRequest
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public string CodeBody { get; set; }
        public string Language { get; set; }
        public bool IsPublic { get; set; }
        public List<string> Tags { get; set; }
    }
}
