using SnippetVault.Domain.Entities;

namespace SnippetVault.Application.Interfaces
{
    public interface ISnippetRepository
    {
        Task<(List<Snippet> Snippets, int TotalCount)> GetAllAsync(
            int page, int pageSize, string? search, string? language, string? tag, Guid? userId);
        Task<Snippet?> GetByIdAsync(Guid id);
        Task<Snippet> CreateAsync(Snippet snippet);
        Task<Snippet> UpdateAsync(Snippet snippet);
        Task SoftDeleteAsync(Guid id);
        Task HardDeleteAsync(Guid id);
    }
}