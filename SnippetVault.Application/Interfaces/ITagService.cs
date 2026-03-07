using SnippetVault.Application.DTOs.Tags;

namespace SnippetVault.Application.Interfaces
{
    public interface ITagService
    {
        Task<List<TagResponse>> GetAllTags(string search);
    }
}
