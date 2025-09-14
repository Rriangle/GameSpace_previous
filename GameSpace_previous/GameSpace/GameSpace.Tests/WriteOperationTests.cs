using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using GameSpace.Data;
using GameSpace.Core.Repositories;
using GameSpace.Infrastructure.Repositories;
using GameSpace.Core.Models;
using Xunit;
using System.Text.Json;

namespace GameSpace.Tests
{
    /// <summary>
    /// 寫入操作測試
    /// </summary>
    public class WriteOperationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public WriteOperationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task SignIn_WithValidRequest_ReturnsSuccess()
        {
            // Arrange
            var client = _factory.CreateClient();
            var request = new SignInRequest
            {
                UserId = 1,
                IdempotencyKey = Guid.NewGuid().ToString(),
                SignInType = "daily"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/signin", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SignInResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            Assert.NotNull(result);
            Assert.True(result.Success);
        }

        [Fact]
        public async Task SignIn_WithDuplicateIdempotencyKey_ReturnsFailure()
        {
            // Arrange
            var client = _factory.CreateClient();
            var idempotencyKey = Guid.NewGuid().ToString();
            var request = new SignInRequest
            {
                UserId = 1,
                IdempotencyKey = idempotencyKey,
                SignInType = "daily"
            };

            // Act - First request
            var response1 = await client.PostAsJsonAsync("/api/signin", request);
            response1.EnsureSuccessStatusCode();

            // Act - Second request with same idempotency key
            var response2 = await client.PostAsJsonAsync("/api/signin", request);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response2.StatusCode);
        }

        [Fact]
        public async Task SignIn_WithInvalidUserId_ReturnsFailure()
        {
            // Arrange
            var client = _factory.CreateClient();
            var request = new SignInRequest
            {
                UserId = 99999, // Non-existent user
                IdempotencyKey = Guid.NewGuid().ToString(),
                SignInType = "daily"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/signin", request);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }
    }

    /// <summary>
    /// 寫入存儲庫測試
    /// </summary>
    public class WriteRepositoryTests : IDisposable
    {
        private readonly GameSpaceDbContext _context;
        private readonly IUserWriteRepository _userWriteRepository;

        public WriteRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<GameSpaceDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new GameSpaceDbContext(options);
            _userWriteRepository = new UserWriteRepository(_context);
        }

        [Fact]
        public async Task ProcessSignInAsync_WithNewUser_CreatesWalletAndStats()
        {
            // Arrange
            var request = new SignInRequest
            {
                UserId = 1,
                IdempotencyKey = Guid.NewGuid().ToString(),
                SignInType = "daily"
            };

            // Add a user first
            _context.Users.Add(new User
            {
                UserID = 1,
                Username = "testuser",
                Email = "test@example.com",
                CreatedAt = DateTime.Now
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _userWriteRepository.ProcessSignInAsync(request);

            // Assert
            Assert.True(result.Success);
            Assert.True(result.PointsEarned > 0);
            Assert.True(result.ExpEarned > 0);

            // Verify wallet was created
            var wallet = await _context.UserWallets.FirstOrDefaultAsync(w => w.UserID == 1);
            Assert.NotNull(wallet);
            Assert.True(wallet.Points > 0);

            // Verify sign-in stats were created
            var stats = await _context.UserSignInStats.FirstOrDefaultAsync(s => s.UserID == 1);
            Assert.NotNull(stats);
            Assert.True(stats.ConsecutiveDays > 0);

            // Verify wallet history was created
            var history = await _context.WalletHistory.FirstOrDefaultAsync(h => h.UserID == 1);
            Assert.NotNull(history);
        }

        [Fact]
        public async Task UpdatePetExpAsync_WithNewPet_CreatesPetAndLevelsUp()
        {
            // Arrange
            var userId = 1;
            var expGained = 150;

            // Act
            var result = await _userWriteRepository.UpdatePetExpAsync(userId, expGained);

            // Assert
            Assert.True(result);

            var pet = await _context.Pets.FirstOrDefaultAsync(p => p.UserID == userId);
            Assert.NotNull(pet);
            Assert.Equal("小夥伴", pet.PetName);
            Assert.True(pet.Level > 1); // Should level up with 150 exp
        }

        [Fact]
        public async Task AddWalletHistoryAsync_WithValidData_CreatesHistoryRecord()
        {
            // Arrange
            var userId = 1;
            var pointsChange = 100;
            var description = "測試交易";
            var transactionType = "test";

            // Act
            var result = await _userWriteRepository.AddWalletHistoryAsync(userId, pointsChange, description, transactionType);

            // Assert
            Assert.True(result);

            var history = await _context.WalletHistory.FirstOrDefaultAsync(h => h.UserID == userId);
            Assert.NotNull(history);
            Assert.Equal(pointsChange, history.PointsChanged);
            Assert.Equal(description, history.Description);
            Assert.Equal(transactionType, history.ChangeType);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
