using Microsoft.EntityFrameworkCore;
using SnippetVault.Application.Interfaces;
using SnippetVault.Domain.Entities;
using SnippetVault.Infrastructure.Data;

namespace SnippetVault.Infrastructure.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly AppDbContext _context;

        public TagRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Tag?> GetByNameAsync(string search)
        {
            return await _context.Tags.FirstOrDefaultAsync(t => t.Name == search.ToLower());
        }

        public async Task<List<Tag>> GetByNamesAsync(List<string> names)
        {
            return await _context.Tags.Where(t => names.Contains(t.Name)).ToListAsync();
        }

        public async Task<Tag> CreateAsync(Tag item)
        {
            await _context.Tags.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<List<Tag>> SearchAsync(string? search)
        {
            var query = _context.Tags.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(t => t.Name.Contains(search));

            return await query.OrderBy(t => t.Name).ToListAsync();
        }
    }
}
