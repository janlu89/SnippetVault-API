using Microsoft.EntityFrameworkCore;
using SnippetVault.Application.Interfaces;
using SnippetVault.Domain.Entities;
using SnippetVault.Infrastructure.Data;

namespace SnippetVault.Infrastructure.Repositories
{
    public class SnippetRepository : ISnippetRepository
    {
        private readonly AppDbContext _context;

        public SnippetRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Snippet> Snippets, int TotalCount)> GetAllAsync(
            int page, int pageSize, string? search, string? language, string? tag, Guid? userId)
        {
            var query = _context.Snippets
                .Include(s => s.User)
                .Include(s => s.SnippetTags)
                    .ThenInclude(st => st.Tag)
                .AsQueryable();

            // Show public snippets + the user's own private ones
            if (userId.HasValue)
                query = query.Where(s => s.IsPublic || s.UserId == userId.Value);
            else
                query = query.Where(s => s.IsPublic);

            // Filter by search text
            if (!string.IsNullOrEmpty(search))
                query = query.Where(s =>
                    s.Title.Contains(search) ||
                    (s.Description != null && s.Description.Contains(search)));

            // Filter by language
            if (!string.IsNullOrEmpty(language))
                query = query.Where(s => s.Language == language);

            // Filter by tag
            if (!string.IsNullOrEmpty(tag))
                query = query.Where(s =>
                    s.SnippetTags.Any(st => st.Tag.Name == tag));

            var totalCount = await query.CountAsync();

            var snippets = await query
                .OrderByDescending(s => s.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (snippets, totalCount);
        }

        public async Task<Snippet?> GetByIdAsync(Guid id)
        {
            return await _context.Snippets
                .Include(s => s.User)
                .Include(s => s.SnippetTags)
                    .ThenInclude(st => st.Tag)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Snippet> CreateAsync(Snippet snippet)
        {
            await _context.Snippets.AddAsync(snippet);
            await _context.SaveChangesAsync();
            return snippet;
        }

        public async Task<Snippet> UpdateAsync(Snippet snippet)
        {
            _context.Snippets.Update(snippet);
            await _context.SaveChangesAsync();
            return snippet;
        }

        public async Task SoftDeleteAsync(Guid id)
        {
            var snippet = await _context.Snippets.FindAsync(id);
            if (snippet == null)
                throw new Exception("Snippet not found");

            snippet.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task HardDeleteAsync(Guid id)
        {
            var snippet = await _context.Snippets
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(s => s.Id == id);
            if (snippet == null)
                throw new Exception("Snippet not found");

            _context.Snippets.Remove(snippet);
            await _context.SaveChangesAsync();
        }
    }
}