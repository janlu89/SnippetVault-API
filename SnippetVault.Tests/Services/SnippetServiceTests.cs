using Moq;
using SnippetVault.Application.DTOs.Snippets;
using SnippetVault.Application.Interfaces;
using SnippetVault.Application.Services;
using SnippetVault.Domain.Entities;

namespace SnippetVault.Tests.Services
{
    public class SnippetServiceTests
    {
        private readonly Mock<ISnippetRepository> _snippetRepositoryMock;
        private readonly Mock<ITagRepository> _tagRepositoryMock;
        private readonly SnippetService _snippetService;

        public SnippetServiceTests()
        {
            _snippetRepositoryMock = new Mock<ISnippetRepository>();
            _tagRepositoryMock = new Mock<ITagRepository>();
            _snippetService = new SnippetService(
                _snippetRepositoryMock.Object,
                _tagRepositoryMock.Object);
        }

        [Fact]
        public async Task GetSnippets_WithValidData_ReturnsSnippetListResponse()
        {
            //Arrange
            var snippets = new List<Snippet>
            {
                new Snippet
                {
                    Id = Guid.NewGuid(),
                    Title = "Test Snippet",
                    Description = "A test snippet",
                    CodeBody = "Console.WriteLine(\"Hello World\");",
                    Language = "C#",
                    IsPublic = true,
                    UserId = Guid.NewGuid(),
                    User = new User { Username = "testuser" },
                    SnippetTags = new List<SnippetTag>(),
                    CreatedAt = DateTime.UtcNow
                }
            };
            _snippetRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<Guid?>()))
                .ReturnsAsync((snippets, snippets.Count));

            //Act
            var result = await _snippetService.GetSnippets(1, 10, null, null, null, null);

            //Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal("Test Snippet", result.Items[0].Title);
        }

        [Fact]
        public async Task GetSnippetById_WithExistingId_ReturnsSnippetResponse()
        {
            //Arrange
            var snippetId = Guid.NewGuid();
            var snippet = new Snippet
            {
                Id = snippetId,
                Title = "Test Snippet",
                Description = "A test snippet",
                CodeBody = "Console.WriteLine(\"Hello World\");",
                Language = "C#",
                IsPublic = true,
                UserId = Guid.NewGuid(),
                User = new User { Username = "testuser" },
                SnippetTags = new List<SnippetTag>(),
                CreatedAt = DateTime.UtcNow
            };

            _snippetRepositoryMock.Setup(repo => repo.GetByIdAsync(snippetId))
                .ReturnsAsync(snippet);

            //Act
            var result = await _snippetService.GetSnippetById(snippetId);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("Test Snippet", result.Title);
        }

        [Fact]
        public async Task GetSnippetById_WithNonExistingId_ThrowsKeyNotFoundException()
        {
            //Arrange
            var snippetId = Guid.NewGuid();
            _snippetRepositoryMock.Setup(repo => repo.GetByIdAsync(snippetId))
                .ReturnsAsync((Snippet?)null);

            //Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _snippetService.GetSnippetById(snippetId));
        }

        [Fact]
        public async Task CreateSnippet_WithValidData_ReturnsSnippetResponse()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var createRequest = new CreateSnippetRequest
            {
                Title = "New Snippet",
                Description = "A new test snippet",
                CodeBody = "Console.WriteLine(\"Hello World\");",
                Language = "C#",
                IsPublic = true,
                Tags = new List<string> { "test", "csharp" }
            };
            _tagRepositoryMock.Setup(repo => repo.GetByNamesAsync(It.IsAny<List<string>>()))
                .ReturnsAsync(new List<Tag>());

            _tagRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Tag>()))
                .ReturnsAsync((Tag t) => t);

            _snippetRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Snippet>()))
                .ReturnsAsync((Snippet s) => s);

            _snippetRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => new Snippet
                {
                    Id = id,
                    Title = "New Snippet",
                    CodeBody = "Console.WriteLine(\"Hello World\");",
                    Language = "C#",
                    IsPublic = true,
                    UserId = userId,
                    User = new User { Username = "testuser" },
                    SnippetTags = new List<SnippetTag>(),
                    CreatedAt = DateTime.UtcNow
                });
            //Act
            var result = await _snippetService.CreateSnippet(createRequest, userId);
            //Assert
            Assert.NotNull(result);
            Assert.Equal("New Snippet", result.Title);
        }

        [Fact]
        public async Task CreateSnippet_WithExistingTags_ReturnsSnippetResponse()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var createRequest = new CreateSnippetRequest
            {
                Title = "New Snippet",
                Description = "A new test snippet",
                CodeBody = "Console.WriteLine(\"Hello World\");",
                Language = "C#",
                IsPublic = true,
                Tags = new List<string> { "test", "csharp" }
            };
            var existingTags = new List<Tag>
            {
                new Tag { Id = Guid.NewGuid(), Name = "test" },
                new Tag { Id = Guid.NewGuid(), Name = "csharp" }
            };
            _tagRepositoryMock.Setup(repo => repo.GetByNamesAsync(It.IsAny<List<string>>()))
                .ReturnsAsync(existingTags);
            _snippetRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => new Snippet
                {
                    Id = id,
                    Title = "New Snippet",
                    CodeBody = "Console.WriteLine(\"Hello World\");",
                    Language = "C#",
                    IsPublic = true,
                    UserId = userId,
                    User = new User { Username = "testuser" },
                    SnippetTags = new List<SnippetTag>(),
                    CreatedAt = DateTime.UtcNow
                });
            //Act
            var result = await _snippetService.CreateSnippet(createRequest, userId);
            //Assert
            Assert.NotNull(result);
            Assert.Equal("New Snippet", result.Title);
        }

        [Fact]
        public async Task UpdateSnippet_WithValidOwner()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var snippetId = Guid.NewGuid();

            var existingSnippet = new Snippet
            {
                Id = snippetId,
                Title = "Old Title",
                CodeBody = "old code",
                Language = "C#",
                UserId = userId,
                User = new User { Username = "testuser" },
                SnippetTags = new List<SnippetTag>(),
                CreatedAt = DateTime.UtcNow
            };

            var updateRequest = new UpdateSnippetRequest
            {
                Title = "Updated Title",
                CodeBody = "new code",
                Language = "C#",
                IsPublic = true,
                Tags = new List<string> { "updated" }
            };

            _snippetRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => new Snippet
                {
                    Id = snippetId,
                    Title = "Updated Title",
                    CodeBody = "new code",
                    Language = "C#",
                    IsPublic = true,
                    UserId = userId,
                    User = new User { Username = "testuser" },
                    SnippetTags = new List<SnippetTag>
                    {
                        new SnippetTag
                        {
                            SnippetId = snippetId,
                            TagId = Guid.NewGuid(),
                            Tag = new Tag { Name = "updated" }
                        }
                    },
                    CreatedAt = DateTime.UtcNow
                });
            _snippetRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Snippet>()))
                .ReturnsAsync((Snippet s) => s);
            _tagRepositoryMock.Setup(repo => repo.GetByNamesAsync(It.IsAny<List<string>>()))
                .ReturnsAsync(new List<Tag>());
            _tagRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Tag>()))
                .ReturnsAsync((Tag t) => t);

            //Act
            var result = await _snippetService.UpdateSnippet(snippetId, updateRequest, userId);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Title", result.Title);
        }

        [Fact]
        public async Task UpdateSnippet_WithWrongOwner_ThrowsUnauthorizedAccessException()
        {
            //Arrange
            var ownerId = Guid.NewGuid();
            var wrongUserId = Guid.NewGuid();
            var snippetId = Guid.NewGuid();

            _snippetRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => new Snippet
                {
                    Id = snippetId,
                    Title = "Test",
                    CodeBody = "code",
                    Language = "C#",
                    UserId = ownerId,
                    User = new User { Username = "owner" },
                    SnippetTags = new List<SnippetTag>(),
                    CreatedAt = DateTime.UtcNow
                });

            var request = new UpdateSnippetRequest
            {
                Title = "Hacked",
                CodeBody = "evil code",
                Language = "C#",
                IsPublic = true,
                Tags = new List<string>()
            };

            //Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _snippetService.UpdateSnippet(snippetId, request, wrongUserId));
        }

        [Fact]
        public async Task DeleteSnippet_WithValidOwner()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var snippetId = Guid.NewGuid();
            _snippetRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => new Snippet
                {
                    Id = snippetId,
                    Title = "Test",
                    CodeBody = "code",
                    Language = "C#",
                    UserId = userId,
                    User = new User { Username = "testuser" },
                    SnippetTags = new List<SnippetTag>(),
                    CreatedAt = DateTime.UtcNow
                });
            _snippetRepositoryMock.Setup(repo => repo.SoftDeleteAsync(snippetId))
                .Returns(Task.CompletedTask);
            //Act
            await _snippetService.DeleteSnippet(snippetId, userId);
            //Assert
            _snippetRepositoryMock.Verify(repo => repo.SoftDeleteAsync(snippetId));
        }

        [Fact]
        public async Task DeleteSnippet_WithWrongOwner_ThrowsUnauthorizedAccessException()
        {
            //Arrange
            var ownerId = Guid.NewGuid();
            var wrongUserId = Guid.NewGuid();
            var snippetId = Guid.NewGuid();
            _snippetRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => new Snippet
                {
                    Id = snippetId,
                    Title = "Test",
                    CodeBody = "code",
                    Language = "C#",
                    UserId = ownerId,
                    User = new User { Username = "owner" },
                    SnippetTags = new List<SnippetTag>(),
                    CreatedAt = DateTime.UtcNow
                });
            //Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _snippetService.DeleteSnippet(snippetId, wrongUserId));
        }
    }
}