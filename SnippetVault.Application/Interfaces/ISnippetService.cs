using SnippetVault.Application.DTOs.Snippets;

namespace SnippetVault.Application.Interfaces
{
    public interface ISnippetService
    {
        Task<SnippetListResponse> GetSnippets(int page, int pageSize, string? search, string? language, string? tag, Guid? userId);
        Task<SnippetResponse> GetSnippetById(Guid id);
        Task<SnippetResponse> CreateSnippet(CreateSnippetRequest request, Guid userId);
        Task<SnippetResponse> UpdateSnippet(Guid id, UpdateSnippetRequest request, Guid userId);
        Task DeleteSnippet(Guid id, Guid userId);
        Task HardDeleteSnippet(Guid id);
    }
}
