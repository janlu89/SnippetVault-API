using SnippetVault.Application.DTOs.Snippets;
using SnippetVault.Application.Interfaces;
using SnippetVault.Domain.Entities;

namespace SnippetVault.Application.Services
{
    public class SnippetService : ISnippetService
    {
        private readonly ISnippetRepository _snippetRepository;
        private readonly ITagRepository _tagRepository;
        public SnippetService(ISnippetRepository snippetRepository, ITagRepository tagRepository)
        {
            _snippetRepository = snippetRepository;
            _tagRepository = tagRepository;
        }

        public async Task<SnippetListResponse> GetSnippets(int page, int pageSize, string? search, string? language, string? tag, Guid? userId)
        {
            var (snippets, totalCount) = await _snippetRepository.GetAllAsync(page, pageSize, search, language, tag, userId);
            
            return new SnippetListResponse
            {
                Items   = snippets.Select(s => MapToResponse(s)).ToList(),
                Page    = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<SnippetResponse> GetSnippetById(Guid id)
        {
            var snippet = await _snippetRepository.GetByIdAsync(id);
            if (snippet == null)
                throw new KeyNotFoundException("Snippet not found");
            return MapToResponse(snippet);
        }

        public async Task<SnippetResponse> CreateSnippet(CreateSnippetRequest request, Guid userId)
        {
            var snippet = new Snippet
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                CodeBody = request.CodeBody,
                Language = request.Language,
                IsPublic = request.IsPublic,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                SnippetTags = new List<SnippetTag>()
            };

            if (request.Tags != null && request.Tags.Any())
            {
                var normalizedNames = request.Tags.Select(t => t.ToLower().Trim()).Distinct().ToList();
                var existingTags = await _tagRepository.GetByNamesAsync(normalizedNames);

                foreach (var tagName in normalizedNames)
                {
                    var tag = existingTags.FirstOrDefault(t => t.Name == tagName);
                    if (tag == null)
                    {
                        tag = await _tagRepository.CreateAsync(new Tag
                        {
                            Id = Guid.NewGuid(),
                            Name = tagName,
                            CreatedAt = DateTime.UtcNow
                        });
                    }

                    snippet.SnippetTags.Add(new SnippetTag
                    {
                        SnippetId = snippet.Id,
                        TagId = tag.Id
                    });
                }
            }

            await _snippetRepository.CreateAsync(snippet);
            var created = await _snippetRepository.GetByIdAsync(snippet.Id);
            return MapToResponse(created!);
        }

        public async Task<SnippetResponse> UpdateSnippet(Guid id, UpdateSnippetRequest request, Guid userId)
        {
            var snippet = await _snippetRepository.GetByIdAsync(id);
            if (snippet == null)
                throw new KeyNotFoundException("Snippet not found");
            if (snippet.UserId != userId)
                throw new UnauthorizedAccessException("You do not have permission to update this snippet");
            snippet.Title = request.Title;
            snippet.Description = request.Description;
            snippet.CodeBody = request.CodeBody;
            snippet.Language = request.Language;
            snippet.IsPublic = request.IsPublic;
            snippet.UpdatedAt = DateTime.UtcNow;

            snippet.SnippetTags.Clear();

            if (request.Tags != null && request.Tags.Any())
            {
                var normalizedNames = request.Tags.Select(t => t.ToLower().Trim()).Distinct().ToList();
                var existingTags = await _tagRepository.GetByNamesAsync(normalizedNames);

                foreach (var tagName in normalizedNames)
                {
                    var tag = existingTags.FirstOrDefault(t => t.Name == tagName);
                    if (tag == null)
                    {
                        tag = await _tagRepository.CreateAsync(new Tag
                        {
                            Id = Guid.NewGuid(),
                            Name = tagName,
                            CreatedAt = DateTime.UtcNow
                        });
                    }

                    snippet.SnippetTags.Add(new SnippetTag
                    {
                        SnippetId = snippet.Id,
                        TagId = tag.Id
                    });
                }
            }

            await _snippetRepository.UpdateAsync(snippet);
            var updated = await _snippetRepository.GetByIdAsync(snippet.Id);
            return MapToResponse(updated!);
        }

        public async Task DeleteSnippet(Guid id, Guid userId)
        {
            var snippet = await _snippetRepository.GetByIdAsync(id);
            if (snippet == null)
                throw new KeyNotFoundException("Snippet not found");
            if (snippet.UserId != userId)
                throw new UnauthorizedAccessException("You do not have permission to delete this snippet");
            await _snippetRepository.SoftDeleteAsync(snippet.Id);
        }

        public async Task HardDeleteSnippet(Guid id)
        {
            var snippet = await _snippetRepository.GetByIdAsync(id);
            if (snippet == null)
                throw new KeyNotFoundException("Snippet not found");
            await _snippetRepository.HardDeleteAsync(snippet.Id);
        }

        private SnippetResponse MapToResponse(Snippet snippet)
        {
            return new SnippetResponse
            {
                Id = snippet.Id,
                Title = snippet.Title,
                Description = snippet.Description,
                CodeBody = snippet.CodeBody,
                Language = snippet.Language,
                IsPublic = snippet.IsPublic,
                Username = snippet.User?.Username ?? "",
                Tags = snippet.SnippetTags?.Select(st => st.Tag.Name).ToList() ?? new List<string>(),
                CreatedAt = snippet.CreatedAt,
                UpdatedAt = snippet.UpdatedAt
            };
        }
    }
}
