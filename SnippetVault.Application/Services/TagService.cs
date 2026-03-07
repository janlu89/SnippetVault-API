using SnippetVault.Application.DTOs.Tags;
using SnippetVault.Application.Interfaces;

namespace SnippetVault.Application.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;
        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }
        public async Task<List<TagResponse>> GetAllTags(string? search)
        {
            var tags = await _tagRepository.SearchAsync(search);
            return tags.Select(t => new TagResponse { Id = t.Id, Name = t.Name }).ToList();
        }
    }
}
