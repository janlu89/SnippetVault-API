using SnippetVault.Application.DTOs.Auth;

namespace SnippetVault.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> Register(RegisterRequest request);
        Task<AuthResponse> Login(LoginRequest request);
    }
}