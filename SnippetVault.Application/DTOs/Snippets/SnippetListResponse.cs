namespace SnippetVault.Application.DTOs.Snippets
{
    public class SnippetListResponse
    {
        public required List<SnippetResponse> Items { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
