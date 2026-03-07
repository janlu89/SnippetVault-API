namespace SnippetVault.Application.DTOs.Auth
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public Guid UserId { get; set; }
        public DateTime Expiration { get; set; }
    }
}
