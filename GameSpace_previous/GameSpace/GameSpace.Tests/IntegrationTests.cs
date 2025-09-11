using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using GameSpace.Data;
using GameSpace.Core.Repositories;
using GameSpace.Infrastructure.Repositories;
using Xunit;

namespace GameSpace.Tests
{
    /// <summary>
    /// 整合測試
    /// </summary>
    public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public IntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task HealthCheck_ReturnsOk()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/health");

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Swagger_ReturnsOk()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/swagger");

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task UsersController_GetUsers_ReturnsOk()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/users");

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PetsController_GetPets_ReturnsOk()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/pets");

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }

    /// <summary>
    /// 存儲庫測試
    /// </summary>
    public class RepositoryTests : IDisposable
    {
        private readonly GameSpaceDbContext _context;
        private readonly IUserReadOnlyRepository _userRepository;

        public RepositoryTests()
        {
            var options = new DbContextOptionsBuilder<GameSpaceDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new GameSpaceDbContext(options);
            _userRepository = new UserReadOnlyRepository(_context);
        }

        [Fact]
        public async Task GetUserCountAsync_ReturnsZero_WhenNoUsers()
        {
            // Act
            var count = await _userRepository.GetUserCountAsync();

            // Assert
            Assert.Equal(0, count);
        }

        [Fact]
        public async Task GetUsersAsync_ReturnsEmptyList_WhenNoUsers()
        {
            // Act
            var users = await _userRepository.GetUsersAsync();

            // Assert
            Assert.Empty(users);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
