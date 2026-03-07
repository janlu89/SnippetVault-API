using Moq;
using SnippetVault.Application.DTOs.Auth;
using SnippetVault.Application.Interfaces;
using SnippetVault.Application.Services;
using SnippetVault.Domain.Entities;

namespace SnippetVault.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _tokenServiceMock = new Mock<ITokenService>();
            _authService = new AuthService(
                _userRepositoryMock.Object,
                _tokenServiceMock.Object);
        }

        [Fact]
        public async Task Register_WithValidData_ReturnsAuthResponse()
        {
            //Arrange
            _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);
            _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);

            _tokenServiceMock.Setup(service => service.GenerateToken(It.IsAny<User>()))
                .Returns("mocked_token");

            //Act
            var registerRequest = new RegisterRequest
            {
                Username = "testuser",
                Email = "testemail@password.com",
                Password = "TestPassword123!"
            };

            var result = await _authService.Register(registerRequest);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("mocked_token", result.Token);
            Assert.Equal("testuser", result.Username);
        }

        [Fact]
        public async Task Register_WithExistingEmail_ThrowsException()
        {
            //Arrange
            var existingUser = new User
            {
                Id = Guid.NewGuid(),
                Username = "existinguser",
                Email = "existing@email.com"
            };

            _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(existingUser);  // returns the user, meaning email already exists

            //Act & Assert
            var request = new RegisterRequest
            {
                Username = "newuser",
                Email = "existing@email.com",
                Password = "TestPassword123!"
            };

            await Assert.ThrowsAsync<Exception>(() => _authService.Register(request));
        }

        [Fact]
        public async Task Register_WithExistingUsername_ThrowsException()
        {
            //Arrange
            var existingUser = new User
            {
                Id = Guid.NewGuid(),
                Username = "existinguser",
                Email = "newemail@new.com"
            };

            _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(existingUser); // returns the user, meaning username already exists

            //Act & Assert
            var request = new RegisterRequest
            {
                Username = "existinguser",
                Email = "newemail@email.com",
                Password = "TestPassword123!"
            };

            await Assert.ThrowsAsync<Exception>(() => _authService.Register(request));
        }


        [Fact]
        public async Task Login_WithValidData_ReturnsAuthResponse()
        {
            //Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Email = "test@email.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("TestPassword123!")
            };

            _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user); // mock returns this user when searching by email
            _tokenServiceMock.Setup(service => service.GenerateToken(It.IsAny<User>()))
                .Returns("mocked_token");

            //Act
            var request = new LoginRequest
            {
                Email = "testuser@email.com",
                Password = "TestPassword123!"
            };

            var result = await _authService.Login(request);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("mocked_token", result.Token);
            Assert.Equal("testuser", result.Username);
        }

        [Fact]
        public async Task Login_WithInvalidEmail_ThrowsException()
        {
            //Arrange
            _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null); // mock returns null, meaning email not found


            //Act
            var request = new LoginRequest
            {
                Email = "notexisting@email.com",
                Password = "TestPassword123!"
            };


            //Assert
            await Assert.ThrowsAsync<Exception>(() => _authService.Login(request));
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ThrowsException()
        {
            //Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Email = "test@email.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("TestPassword123!")
            };

            _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            //Act & Assert
            var request = new LoginRequest
            {
                Email = "test@email.com",
                Password = "WrongPassword456!"
            };
            await Assert.ThrowsAsync<Exception>(() => _authService.Login(request));
        }
    }
}