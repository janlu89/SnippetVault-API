namespace SnippetVault.Domain.Entities
{
    public class SnippetTag
    {
        public Snippet Snippet { get; set; }
        public Guid SnippetId { get; set; }    
        public Tag Tag { get; set; }
        public Guid TagId { get; set; }
    }
}
