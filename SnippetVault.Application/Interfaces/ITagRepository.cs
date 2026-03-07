using SnippetVault.Domain.Entities;

namespace SnippetVault.Application.Interfaces
{
    public interface ITagRepository
    {
        Task<Tag?> GetByNameAsync(string search);
        Task<List<Tag>> GetByNamesAsync(List<string> names);
        Task<Tag> CreateAsync(Tag item);
        Task<List<Tag>> SearchAsync(string? search);
    }
}
