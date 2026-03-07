using SnippetVault.Domain.Entities;

namespace SnippetVault.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}