using SnippetVault.Application.DTOs.Auth;
using SnippetVault.Application.Interfaces;
using SnippetVault.Domain.Entities;

namespace SnippetVault.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        public AuthService(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<AuthResponse> Login(LoginRequest request)
        {
            var existingEmail = await _userRepository.GetByEmailAsync(request.Email);
            if (existingEmail == null)
                throw new Exception("Invalid email or password");

            var validPassword = BCrypt.Net.BCrypt.Verify(request.Password, existingEmail.PasswordHash);
            if (!validPassword)
                throw new Exception("Invalid email or password");

            return new AuthResponse
            {
                Token = _tokenService.GenerateToken(existingEmail),
                Username = existingEmail.Username,
                UserId = existingEmail.Id,
                Expiration = DateTime.UtcNow.AddMinutes(60)
            };
        }

        public async Task<AuthResponse> Register(RegisterRequest request)
        {
            var existingEmail = await _userRepository.GetByEmailAsync(request.Email);
            if (existingEmail != null)
                throw new Exception("Email already exists");

            var existingUsername = await _userRepository.GetByUsernameAsync(request.Username);
            if (existingUsername != null)
                throw new Exception("Username already exists");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.CreateAsync(user);

            var token = _tokenService.GenerateToken(user);

            return new AuthResponse
            {
                Token = token,
                Username = user.Username,
                UserId = user.Id,
                Expiration = DateTime.UtcNow.AddMinutes(60)
            };
        }
    }
}
